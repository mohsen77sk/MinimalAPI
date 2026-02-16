using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Minimal.Api.Exceptions;
using Minimal.Api.Features.BankAccounts.Models;
using Minimal.Api.Features.BankAccounts.Profiles;
using Minimal.DataAccess;

namespace Minimal.Api.Features.BankAccounts.Queries;

public class GetBankAccountByIdHandler : IRequestHandler<GetBankAccountById, BankAccountGetDto>
{
    private readonly ApplicationDbContext _context;
    private readonly BankAccountMapper _mapper;
    private readonly IStringLocalizer _localizer;

    public GetBankAccountByIdHandler(ApplicationDbContext context, BankAccountMapper mapper, IStringLocalizer<SharedResource> localizer)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
    }

    public async Task<BankAccountGetDto> Handle(GetBankAccountById request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var BankAccount = await _context.BankAccounts
            .AsNoTracking()
            .Include(ba => ba.Bank)
            .Include(ba => ba.Person)
            .FirstOrDefaultAsync(ba => ba.Id == request.BankAccountId, cancellationToken);

        if (BankAccount is null)
        {
            throw new NotFoundException(_localizer.GetString("notFoundBankAccount").Value);
        }

        return _mapper.MapToBankAccountGetDto(BankAccount);
    }
}