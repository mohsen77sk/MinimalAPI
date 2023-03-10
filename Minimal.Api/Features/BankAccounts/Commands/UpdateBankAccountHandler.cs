using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Minimal.Api.Exceptions;
using Minimal.Api.Features.BankAccounts.Models;
using Minimal.DataAccess;
using Minimal.Domain;

namespace Minimal.Api.Features.BankAccounts.Commands;

public class UpdateBankAccountHandler : IRequestHandler<UpdateBankAccount, BankAccountGetDto>
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer _localizer;

    public UpdateBankAccountHandler(ApplicationDbContext context, IMapper mapper, IStringLocalizer<SharedResource> localizer)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
    }

    public async Task<BankAccountGetDto> Handle(UpdateBankAccount request, CancellationToken cancellationToken)
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

        var person = await _context.People.FirstOrDefaultAsync(p => p.Id.Equals(request.PersonId), cancellationToken);
        if (person is null)
        {
            throw new ValidationException(nameof(request.PersonId), _localizer.GetString("notFound").Value);
        }

        var bank = await _context.Banks.FirstOrDefaultAsync(b => b.Id.Equals(request.BankId), cancellationToken);
        if (bank is null)
        {
            throw new ValidationException(nameof(request.BankId), _localizer.GetString("notFound").Value);
        }

        bankAccount.Person = person;
        bankAccount.Bank = bank;
        bankAccount.BranchCode = request.BranchCode;
        bankAccount.BranchName = request.BranchName;
        bankAccount.AccountNumber = request.AccountNumber;
        bankAccount.CardNumber = request.CardNumber;
        bankAccount.Iban = request.Iban;

        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<BankAccountGetDto>(bankAccount);
    }
}