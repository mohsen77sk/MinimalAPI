using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
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

    public CreateAccountTransactionHandler(ApplicationDbContext context, AccountTransactionMapper mapper, IStringLocalizer<SharedResource> localizer)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
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

        var documentToAdd = new Document
        {
            Date = request.Date,
            Note = request.Note,
            FiscalYear = await _context.FiscalYears.SingleAsync(f => f.Id == 1, cancellationToken),
            DocumentType = await _context.DocumentTypes.SingleAsync(dt => dt.Code == (request.TransactionType == TransactionTypeEnum.Deposit ? "12" : "13"), cancellationToken),
            DocumentItems =
            [
                new DocumentArticle
                {
                    AccountSubsid = await _context.AccountSubsids.SingleAsync(x => x.Code == sourceAccount.AccountType.Code, cancellationToken),
                    AccountDetail = sourceAccount.AccountDetail,
                    Credit = request.TransactionType == TransactionTypeEnum.Deposit ? request.Amount : 0,
                    Debit = request.TransactionType != TransactionTypeEnum.Deposit ? request.Amount : 0,
                    Note = ""
                },
            ],
            IsActive = true,
        };

        // Add the destination account if the transaction type is Transfer
        if (destinationAccount is null)
        {
            documentToAdd.DocumentItems.Add(new DocumentArticle
            {
                AccountSubsid = await _context.AccountSubsids.SingleAsync(x => x.Code == "1101", cancellationToken),
                Debit = request.TransactionType == TransactionTypeEnum.Deposit ? request.Amount : 0,
                Credit = request.TransactionType != TransactionTypeEnum.Deposit ? request.Amount : 0,
                Note = ""
            });
        }
        else
        {
            documentToAdd.DocumentItems.Add(new DocumentArticle
            {
                AccountSubsid = await _context.AccountSubsids.SingleAsync(x => x.Code == destinationAccount.AccountType.Code, cancellationToken),
                AccountDetail = destinationAccount.AccountDetail,
                Debit = request.TransactionType == TransactionTypeEnum.Deposit ? request.Amount : 0,
                Credit = request.TransactionType != TransactionTypeEnum.Deposit ? request.Amount : 0,
                Note = ""
            });
        }

        _context.Documents.Add(documentToAdd);

        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.MapToAccountTransactionGetDto(documentToAdd);
    }
}