using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Minimal.Api.Common.Accounting;
using Minimal.Api.Common.Accounting.Validators;
using Minimal.Api.Exceptions;
using Minimal.Api.Features.Accounts.Models;
using Minimal.Api.Features.Accounts.Profiles;
using Minimal.DataAccess;
using Minimal.Domain;

namespace Minimal.Api.Features.Accounts.Commands;

public class CreateAccountHandler : IRequestHandler<CreateAccount, AccountGetDto>
{
    private readonly ApplicationDbContext _context;
    private readonly AccountMapper _mapper;
    private readonly IStringLocalizer _localizer;
    private readonly DocumentValidator _documentValidator;

    public CreateAccountHandler(
        ApplicationDbContext context,
        AccountMapper mapper,
        IStringLocalizer<SharedResource> localizer,
        DocumentValidator documentValidator)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
        _documentValidator = documentValidator ?? throw new ArgumentNullException(nameof(documentValidator));
    }

    public async Task<AccountGetDto> Handle(CreateAccount request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var accountToAdd = _mapper.MapToAccount(request);

        var accountType = await _context.AccountTypes.FirstOrDefaultAsync(at => at.Id.Equals(request.AccountTypeId), cancellationToken);
        if (accountType is null)
        {
            throw new ValidationException(nameof(request.AccountTypeId), _localizer.GetString("notFound").Value);
        }

        if (accountType.IsActive is false)
        {
            throw new ValidationException(nameof(request.AccountTypeId), _localizer.GetString("accountTypeIsNotActive").Value);
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
            Title = $"حساب {accountToAdd.Code}",
            Account = accountToAdd,
            AccountCategory = await _context.GetAccountCategoryByCodeAsync("2", cancellationToken),
            IsActive = true
        };
        _context.AccountDetails.Add(accountDetailToAdd);

        var documentToAdd = new Document
        {
            Date = accountToAdd.CreateDate,
            FiscalYear = await _context.GetCurrentFiscalYearAsync(cancellationToken),
            DocumentType = await _context.GetDocumentTypeByCodeAsync("10", cancellationToken),
            DocumentItems =
            [
                new DocumentArticle
                {
                    AccountSubsid = await _context.GetAccountSubsidByCodeAsync(accountType.Code, cancellationToken),
                    AccountDetail = accountDetailToAdd,
                    Credit = request.InitCredit,
                    Debit = 0,
                    Note = ""
                },
                new DocumentArticle
                {
                    AccountSubsid = await _context.GetBankAccountAsync(cancellationToken),
                    Credit = 0,
                    Debit = request.InitCredit,
                    Note = ""
                }
            ],
            Note = string.Empty,
            IsActive = true,
        };

        var validation = _documentValidator.ValidateDocument(documentToAdd);
        if (!validation.IsValid)
        {
            throw new ErrorException(validation.ErrorMessage);
        }

        _context.Documents.Add(documentToAdd);

        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.MapToAccountGetDto(accountToAdd);
    }
}