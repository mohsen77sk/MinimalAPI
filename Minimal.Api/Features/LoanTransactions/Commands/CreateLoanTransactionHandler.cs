using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Minimal.Api.Common.Accounting;
using Minimal.Api.Common.Accounting.Validators;
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
    private readonly DocumentValidator _documentValidator;

    public CreateLoanTransactionHandler(
        ApplicationDbContext context,
        LoanTransactionMapper mapper,
        IStringLocalizer<SharedResource> localizer,
        DocumentValidator documentValidator)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
        _documentValidator = documentValidator ?? throw new ArgumentNullException(nameof(documentValidator));
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

        var remaining = await _context.GetAccountBalanceAsync(loan.AccountDetail.Id, cancellationToken);

        if (remaining < request.Amount)
        {
            throw new ValidationException(nameof(request.Amount), _localizer.GetString("transactionAmountBiggerThanRemaining").Value);
        }

        var documentToAdd = new Document
        {
            Date = request.Date,
            Note = request.Note,
            FiscalYear = await _context.GetCurrentFiscalYearAsync(cancellationToken),
            DocumentType = await _context.GetDocumentTypeByCodeAsync("22", cancellationToken),
            DocumentItems =
            [
                new DocumentArticle
                {
                    AccountSubsid = await _context.GetAccountSubsidByCodeAsync(loan.LoanType.Code, cancellationToken),
                    AccountDetail = loan.AccountDetail,
                    Credit = request.Amount,
                    Debit = 0,
                    Note = ""
                },
                new DocumentArticle
                {
                    AccountSubsid = await _context.GetBankAccountAsync(cancellationToken),
                    Credit = 0,
                    Debit = request.Amount,
                    Note = ""
                }
            ],
            IsActive = true,
        };

        var validation = _documentValidator.ValidateDocument(documentToAdd);
        if (!validation.IsValid)
        {
            throw new ErrorException(validation.ErrorMessage);
        }

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