using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Minimal.Api.Exceptions;
using Minimal.Api.Features.BankAccounts.Models;
using Minimal.DataAccess;
using Minimal.Domain;

namespace Minimal.Api.Features.BankAccounts.Commands;

public class CreateBankAccountHandler : IRequestHandler<CreateBankAccount, BankAccountGetDto>
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer _localizer;

    public CreateBankAccountHandler(ApplicationDbContext context, IMapper mapper, IStringLocalizer<SharedResource> localizer)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _localizer = localizer ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<BankAccountGetDto> Handle(CreateBankAccount request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var bankAccountToAdd = _mapper.Map<BankAccount>(request);

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

        bankAccountToAdd.Person = person;
        bankAccountToAdd.Bank = bank;
        bankAccountToAdd.IsActive = true;

        _context.BankAccounts.Add(bankAccountToAdd);
        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<BankAccountGetDto>(bankAccountToAdd);
    }
}