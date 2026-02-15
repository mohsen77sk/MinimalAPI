using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Minimal.Api.Exceptions;
using Minimal.Api.Features.BankAccounts.Models;
using Minimal.Api.Features.BankAccounts.Profiles;
using Minimal.DataAccess;
using Minimal.Domain;

namespace Minimal.Api.Features.BankAccounts.Commands;

public class UpdateStatusBankAccountHandler : IRequestHandler<UpdateStatusBankAccount, BankAccountGetDto>
{
    private readonly ApplicationDbContext _context;
    private readonly BankAccountMapper _mapper;
    private readonly IStringLocalizer _localizer;

    public UpdateStatusBankAccountHandler(ApplicationDbContext context, BankAccountMapper mapper, IStringLocalizer<SharedResource> localizer)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
    }

    public async Task<BankAccountGetDto> Handle(UpdateStatusBankAccount request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var bankAccount = await _context.BankAccounts.FirstOrDefaultAsync(ba => ba.Id == request.Id, cancellationToken);
        if (bankAccount is null)
        {
            throw new NotFoundException(_localizer.GetString("notFoundBankAccount").Value);
        }

        bankAccount.IsActive = request.IsActive;

        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.MapToBankAccountGetDto(bankAccount);
    }
}