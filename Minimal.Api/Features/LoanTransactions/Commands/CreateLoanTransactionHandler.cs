using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Minimal.Api.Exceptions;
using Minimal.Api.Features.LoanTransactions.Models;
using Minimal.Api.Features.LoanTransactions.Profiles;
using Minimal.DataAccess;
using Minimal.Domain;

namespace Minimal.Api.Features.LoanTransactions.Commands;

public class CreateLoanTransactionHandler : IRequestHandler<CreateLoanTransaction, LoanTransactionGetDto>
{
    private readonly ApplicationDbContext _context;
    private readonly LoanTransactionMapper _mapper;
    private readonly IStringLocalizer _localizer;

    public CreateLoanTransactionHandler(ApplicationDbContext context, LoanTransactionMapper mapper, IStringLocalizer<SharedResource> localizer)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
    }

    public async Task<LoanTransactionGetDto> Handle(CreateLoanTransaction request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var loan = await _context.Loans
            .Include(a => a.LoanType)
            .Include(a => a.AccountDetail)
            .FirstOrDefaultAsync(a => a.Id == request.LoanId, cancellationToken);
        if (loan is null)
        {
            throw new NotFoundException();
        }

        if (loan.IsActive is false)
        {
            throw new ValidationException(nameof(request.LoanId), _localizer.GetString("loanIsNotActive").Value);
        }

        if (loan.CreateDate > request.Date)
        {
            throw new ValidationException(nameof(request.Date), _localizer.GetString("transactionDateIsEarlierOpeningDate").Value);
        }

        var remaining = await _context.DocumentArticles
            .AsNoTracking()
            .Where(da => da.AccountDetailId == loan.AccountDetail.Id && da.Document.IsActive == true)
            .SumAsync(da => da.Debit - da.Credit, cancellationToken);

        if (remaining < request.Amount)
        {
            throw new ValidationException(nameof(request.Amount), _localizer.GetString("transactionAmountBiggerThanRemaining").Value);
        }

        var documentToAdd = new Document
        {
            Date = request.Date,
            Note = request.Note,
            FiscalYear = await _context.FiscalYears.SingleAsync(f => f.Id == 1, cancellationToken),
            DocumentType = await _context.DocumentTypes.SingleAsync(at => at.Code == "22", cancellationToken),
            DocumentItems =
            [
                new DocumentArticle
                {
                    AccountSubsid = await _context.AccountSubsids.SingleAsync(x => x.Code == loan.LoanType.Code, cancellationToken),
                    AccountDetail = loan.AccountDetail,
                    Credit = request.Amount,
                    Debit = 0,
                    Note = ""
                },
                new DocumentArticle
                {
                    AccountSubsid = await _context.AccountSubsids.SingleAsync(x => x.Code == "1101", cancellationToken),
                    Credit = 0,
                    Debit = request.Amount,
                    Note = ""
                }
            ],
            IsActive = true,
        };
        _context.Documents.Add(documentToAdd);

        if (remaining == request.Amount)
        {
            loan.CloseDate = request.Date;
            loan.IsActive = false;
            loan.AccountDetail.IsActive = false;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.MapToLoanTransactionGetDto(documentToAdd);
    }
}