using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Minimal.Api.Common.Accounting;
using Minimal.Api.Common.Accounting.Validators;
using Minimal.Api.Exceptions;
using Minimal.Api.Features.AccountTransactions.Models;
using Minimal.Api.Features.AccountTransactions.Profiles;
using Minimal.DataAccess;
using Minimal.Domain;

namespace Minimal.Api.Features.AccountTransactions.Commands;

public class CreateAccountTransactionHandler : IRequestHandler<CreateAccountTransaction, AccountTransactionGetDto>
{
    private readonly ApplicationDbContext _context;
    private readonly AccountTransactionMapper _mapper;
    private readonly IStringLocalizer _localizer;
    private readonly DocumentValidator _documentValidator;

    public CreateAccountTransactionHandler(
        ApplicationDbContext context,
        AccountTransactionMapper mapper,
        IStringLocalizer<SharedResource> localizer,
        DocumentValidator documentValidator)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
        _documentValidator = documentValidator ?? throw new ArgumentNullException(nameof(documentValidator));
    }

    public async Task<AccountTransactionGetDto> Handle(CreateAccountTransaction request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var sourceAccount = await _context.Accounts
            .Include(a => a.AccountType)
            .Include(a => a.AccountDetail)
            .FirstOrDefaultAsync(a => a.Id == request.SourceAccountId, cancellationToken);

        if (sourceAccount is null)
        {
            throw new ValidationException(nameof(request.SourceAccountId), _localizer.GetString("notFoundSourceAccount").Value);
        }

        if (sourceAccount.IsActive is false)
        {
            throw new ValidationException(nameof(request.SourceAccountId), _localizer.GetString("sourceAccountIsNotActive").Value);
        }

        if (sourceAccount.CreateDate > request.Date)
        {
            throw new ValidationException(nameof(request.Date), _localizer.GetString("transactionDateIsEarlierOpeningDate").Value);
        }

        if (request.TransactionType != TransactionTypeEnum.Deposit)
        {
            var balance = await _context.GetAccountDetailBalanceAsync(sourceAccount.AccountDetail.Id, cancellationToken);

            if (balance < request.Amount)
            {
                throw new ValidationException(nameof(request.Amount), _localizer.GetString("transactionAmountBiggerThanBalance").Value);
            }
        }

        Account? destinationAccount = null;

        if (request.TransactionType == TransactionTypeEnum.Transfer)
        {
            destinationAccount = await _context.Accounts
                .Include(a => a.AccountType)
                .Include(a => a.AccountDetail)
                .FirstOrDefaultAsync(a => a.Id == request.DestinationAccountId, cancellationToken);

            if (destinationAccount is null)
            {
                throw new ValidationException(nameof(request.DestinationAccountId), _localizer.GetString("notFoundDestinationAccount").Value);
            }

            if (destinationAccount.IsActive is false)
            {
                throw new ValidationException(nameof(request.DestinationAccountId), _localizer.GetString("destinationAccountIsNotActive").Value);
            }

            if (destinationAccount.CreateDate > request.Date)
            {
                throw new ValidationException(nameof(request.Date), _localizer.GetString("transactionDateIsEarlierOpeningDate").Value);
            }
        }

        string documentTypeCode =
            request.TransactionType == TransactionTypeEnum.Transfer ? "14" :
                request.TransactionType == TransactionTypeEnum.Deposit ? "12" : "13";

        var fiscalYear = await _context.GetCurrentFiscalYearAsync(cancellationToken);
        var documentType = await _context.GetDocumentTypeByCodeAsync(documentTypeCode, cancellationToken);
        var bankAccountSubsid = await _context.GetBankAccountAsync(cancellationToken);
        var sourceAccountSubsid = await _context.GetAccountSubsidByCodeAsync(sourceAccount.AccountType.Code, cancellationToken);

        var documentBuilder = new AccountingDocumentBuilder(fiscalYear, documentType, request.Date, request.Note);

        _ = request.TransactionType == TransactionTypeEnum.Deposit ?
            documentBuilder.Credit(request.Amount, sourceAccountSubsid, sourceAccount.AccountDetail) :
            documentBuilder.Debit(request.Amount, sourceAccountSubsid, sourceAccount.AccountDetail);

        // Add the destination account if the transaction type is Transfer
        if (destinationAccount is not null)
        {
            var destinationAccountSubsid = await _context.GetAccountSubsidByCodeAsync(destinationAccount.AccountType.Code, cancellationToken);

            _ = request.TransactionType == TransactionTypeEnum.Deposit ?
                documentBuilder.Debit(request.Amount, destinationAccountSubsid, destinationAccount.AccountDetail) :
                documentBuilder.Credit(request.Amount, destinationAccountSubsid, destinationAccount.AccountDetail);
        }
        else
        {
            _ = request.TransactionType == TransactionTypeEnum.Deposit ?
                documentBuilder.Debit(request.Amount, bankAccountSubsid) :
                documentBuilder.Credit(request.Amount, bankAccountSubsid);
        }

        var documentToAdd = documentBuilder.Build();

        var validation = _documentValidator.ValidateDocument(documentToAdd);
        if (!validation.IsValid)
        {
            throw new ErrorException(validation.ErrorMessage);
        }

        _context.Documents.Add(documentToAdd);

        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.MapToAccountTransactionGetDto(documentToAdd.DocumentItems.First(di => di.AccountDetailId == sourceAccount.AccountDetail.Id));
    }
}