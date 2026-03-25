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

public class CloseAccountHandler : IRequestHandler<CloseAccount, AccountGetDto>
{
    private readonly ApplicationDbContext _context;
    private readonly AccountMapper _mapper;
    private readonly IStringLocalizer _localizer;
    private readonly DocumentValidator _documentValidator;

    public CloseAccountHandler(
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

    public async Task<AccountGetDto> Handle(CloseAccount request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var account = await _context.Accounts
            .Include(a => a.People)
            .Include(a => a.AccountType)
            .Include(a => a.AccountDetail)
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);
        if (account is null)
        {
            throw new NotFoundException(_localizer.GetString("notFoundAccount").Value);
        }

        if (account.IsActive is false)
        {
            throw new ErrorException(_localizer.GetString("accountIsNotActive").Value);
        }

        if (account.CreateDate >= request.CloseDate)
        {
            throw new ValidationException(nameof(request.CloseDate), _localizer.GetString("closingAccountDateIsBeforeOpeningDate").Value);
        }

        if (await _context.DocumentArticles.AnyAsync(dr =>
            dr.AccountDetailId == account.AccountDetail.Id &&
            dr.Document.Date >= request.CloseDate, cancellationToken))
        {
            throw new ValidationException(nameof(request.CloseDate), _localizer.GetString("closingAccountDateIsBeforeTransaction").Value);
        }

        var fiscalYear = await _context.GetCurrentFiscalYearAsync(cancellationToken);
        var closeAccountDocumentType = await _context.GetDocumentTypeByCodeAsync("11", cancellationToken);
        var bankAccountSubsid = await _context.GetBankAccountAsync(cancellationToken);
        var accountAccountSubsid = await _context.GetAccountSubsidByCodeAsync(account.AccountType.Code, cancellationToken);
        var accountBalance = await _context.GetAccountDetailBalanceAsync(account.AccountDetail.Id, cancellationToken);

        var documentToAdd = new AccountingDocumentBuilder(fiscalYear, closeAccountDocumentType, request.CloseDate)
            .Debit(accountBalance, accountAccountSubsid, account.AccountDetail)
            .Credit(accountBalance, bankAccountSubsid)
            .Build();

        var validation = _documentValidator.ValidateDocument(documentToAdd);
        if (!validation.IsValid)
        {
            throw new ErrorException(validation.ErrorMessage);
        }

        _context.Documents.Add(documentToAdd);

        account.CloseDate = request.CloseDate;
        account.IsActive = false;
        account.AccountDetail.IsActive = false;

        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.MapToAccountGetDto(account);
    }
}