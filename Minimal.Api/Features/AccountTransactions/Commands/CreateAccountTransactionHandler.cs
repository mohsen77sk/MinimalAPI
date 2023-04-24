using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Minimal.Api.Exceptions;
using Minimal.Api.Features.AccountTransactions.Models;
using Minimal.DataAccess;
using Minimal.Domain;

namespace Minimal.Api.Features.AccountTransactions.Commands;

public class CreateAccountTransactionHandler : IRequestHandler<CreateAccountTransaction, AccountTransactionGetDto>
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer _localizer;

    public CreateAccountTransactionHandler(ApplicationDbContext context, IMapper mapper, IStringLocalizer<SharedResource> localizer)
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

        var account = await _context.Accounts
            .Include(a => a.AccountType)
            .Include(a => a.AccountDetail)
            .FirstOrDefaultAsync(a => a.Id == request.AccountId, cancellationToken);
        if (account is null)
        {
            throw new NotFoundException();
        }

        if (account.IsActive is false)
        {
            throw new ValidationException(nameof(request.AccountId), _localizer.GetString("accountIsNotActive").Value);
        }

        if (account.CreateDate > request.Date)
        {
            throw new ValidationException(nameof(request.Date), _localizer.GetString("transactionDateIsEarlierOpeningDate").Value);
        }

        var documentToAdd = new Document
        {
            Date = request.Date,
            Note = request.Note,
            FiscalYear = await _context.FiscalYears.SingleAsync(f => f.Id == 1, cancellationToken),
            DocumentType = request.TransactionType == TransactionTypeEnum.Credit ?
                await _context.DocumentTypes.SingleAsync(at => at.Code == "12", cancellationToken) :
                await _context.DocumentTypes.SingleAsync(at => at.Code == "13", cancellationToken),
            DocumentItems = new List<DocumentArticle>()
            {
                new DocumentArticle
                {
                    AccountSubsid = await _context.AccountSubsids.SingleAsync(x => x.Code == account.AccountType.Code, cancellationToken),
                    AccountDetail = account.AccountDetail,
                    Credit = request.TransactionType == TransactionTypeEnum.Credit ? request.Amount : 0,
                    Debit = request.TransactionType == TransactionTypeEnum.Debit ? request.Amount : 0,
                    Note = ""
                },
                new DocumentArticle
                {
                    AccountSubsid = await _context.AccountSubsids.SingleAsync(x => x.Code == "1101", cancellationToken),
                    Debit = request.TransactionType == TransactionTypeEnum.Credit ? request.Amount : 0,
                    Credit = request.TransactionType == TransactionTypeEnum.Debit ? request.Amount : 0,
                    Note = ""
                }
            },
            IsActive = true,
        };
        _context.Documents.Add(documentToAdd);

        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<AccountTransactionGetDto>(documentToAdd);
    }
}