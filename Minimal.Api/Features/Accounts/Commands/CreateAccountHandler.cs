using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Minimal.Api.Exceptions;
using Minimal.Api.Features.Accounts.Models;
using Minimal.DataAccess;
using Minimal.Domain;

namespace Minimal.Api.Features.Accounts.Commands;

public class CreateAccountHandler : IRequestHandler<CreateAccount, AccountGetDto>
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer _localizer;

    public CreateAccountHandler(ApplicationDbContext context, IMapper mapper, IStringLocalizer<SharedResource> localizer)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
    }

    public async Task<AccountGetDto> Handle(CreateAccount request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var accountToAdd = _mapper.Map<Account>(request);

        var accountType = await _context.AccountTypes.FirstOrDefaultAsync(at => at.Id.Equals(request.AccountTypeId), cancellationToken);
        if (accountType is null)
        {
            throw new ValidationException(nameof(request.AccountTypeId), _localizer.GetString("notFound").Value);
        }

        foreach (var personId in request.PersonId)
        {
            var person = await _context.People.FirstOrDefaultAsync(p => p.Id.Equals(personId), cancellationToken);
            if (person is null)
            {
                throw new ValidationException(nameof(request.PersonId), _localizer.GetString("notFoundPerson").Value);
            }
            if (person.IsActive is false)
            {
                throw new ValidationException(nameof(request.PersonId), _localizer.GetString("personIsNotActive").Value);
            }
            accountToAdd.People.Add(person);
        }

        accountToAdd.AccountType = accountType;
        accountToAdd.IsActive = true;
        _context.Accounts.Add(accountToAdd);

        var accountDetailToAdd = new AccountDetail
        {
            Title = "حساب" + " " + accountToAdd.Code,
            Account = accountToAdd,
            AccountCategory = await _context.AccountCategories.SingleAsync(ac => ac.Code == "2", cancellationToken),
            IsActive = true
        };
        _context.AccountDetails.Add(accountDetailToAdd);

        var documentToAdd = new Document
        {
            Date = accountToAdd.CreateDate,
            Note = "سند افتتاح حساب" + " " + accountToAdd.Code,
            FiscalYear = await _context.FiscalYears.SingleAsync(f => f.Id == 1, cancellationToken),
            DocumentType = await _context.DocumentTypes.SingleAsync(dt => dt.Code == "10", cancellationToken),
            DocumentItems = new List<DocumentArticle>()
            {
                new DocumentArticle
                {
                    AccountSubsid = await _context.AccountSubsids.SingleAsync(x => x.Code == accountType.Code, cancellationToken),
                    AccountDetail = accountDetailToAdd,
                    Credit = request.InitCredit,
                    Debit = 0,
                    Note = ""
                },
                new DocumentArticle
                {
                    AccountSubsid = await _context.AccountSubsids.SingleAsync(x => x.Code == "1101", cancellationToken),
                    AccountDetail = await _context.AccountDetails.SingleAsync(x => x.Code == "11010001", cancellationToken),
                    Credit = 0,
                    Debit = request.InitCredit,
                    Note = ""
                }
            },
            IsActive = true,
        };
        _context.Documents.Add(documentToAdd);

        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<AccountGetDto>(accountToAdd);
    }
}