using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Minimal.Api.Exceptions;
using Minimal.Api.Features.LoanTransactions.Models;
using Minimal.DataAccess;
using Minimal.Domain;

namespace Minimal.Api.Features.LoanTransactions.Commands;

public class UpdateLoanTransactionHandler : IRequestHandler<UpdateLoanTransaction, LoanTransactionGetDto>
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer _localizer;

    public UpdateLoanTransactionHandler(ApplicationDbContext context, IMapper mapper, IStringLocalizer<SharedResource> localizer)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
    }

    public async Task<LoanTransactionGetDto> Handle(UpdateLoanTransaction request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var loan = await _context.Loans
            .Include(a => a.AccountDetail)
            .FirstOrDefaultAsync(a => a.Id == request.LoanId, cancellationToken);
        if (loan is null)
        {
            throw new NotFoundException(_localizer.GetString("notFoundLoan").Value);
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
            .Where(da => da.DocumentId != request.Id && da.AccountDetailId == loan.AccountDetail.Id && da.Document.IsActive == true)
            .SumAsync(da => da.Debit - da.Credit, cancellationToken);
        if (remaining < request.Amount)
        {
            throw new ValidationException(nameof(request.Amount), _localizer.GetString("transactionAmountBiggerThanRemaining").Value);
        }

        var document = await _context.Documents
            .Include(d => d.DocumentType)
            .Include(d => d.DocumentItems)
            .ThenInclude(d => d.AccountDetail)
            .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);
        if (document is null)
        {
            throw new NotFoundException(_localizer.GetString("notFoundTransaction").Value);
        }

        if (document.IsActive is false)
        {
            throw new ErrorException(_localizer.GetString("transactionIsNotActive").Value);
        }

        if (document.DocumentType.Code != "22")
        {
            throw new ErrorException(_localizer.GetString("transactionCanNotBeEdited").Value);
        }

        if (document.DocumentItems.Any(x => x.AccountDetail?.IsActive == false))
        {
            throw new ErrorException(_localizer.GetString("transactionCanNotBeEdited").Value);
        }

        if (!document.DocumentItems.Any(da => da.AccountDetailId == loan.AccountDetail.Id))
        {
            throw new Exception("Error");
        }

        document.Date = request.Date;
        document.Note = request.Note;

        var da1 = document.DocumentItems.Single(da => da.AccountDetailId == loan.AccountDetail.Id);
        da1.Credit = request.Amount;
        da1.Debit = 0;

        var da2 = document.DocumentItems.Single(da => da.AccountDetailId != loan.AccountDetail.Id);
        da2.Credit = 0;
        da2.Debit = request.Amount;

        if (remaining == request.Amount)
        {
            var maxInstalmentDate = await _context.Documents
                .Where(d => d.IsActive == true && d.DocumentItems.Any(da => da.AccountDetailId == loan.AccountDetail.Id))
                .MaxAsync(d => d.Date);
            if (maxInstalmentDate < request.Date) maxInstalmentDate = request.Date;

            loan.CloseDate = maxInstalmentDate;
            loan.IsActive = false;
            loan.AccountDetail.IsActive = false;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<LoanTransactionGetDto>(document);
    }
}