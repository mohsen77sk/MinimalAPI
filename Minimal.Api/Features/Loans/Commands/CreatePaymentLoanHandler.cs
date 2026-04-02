using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Minimal.Api.Common.Accounting;
using Minimal.Api.Common.Accounting.Validators;
using Minimal.Api.Exceptions;
using Minimal.Api.Features.Loans.Models;
using Minimal.Api.Features.Loans.Profiles;
using Minimal.DataAccess;
using Minimal.Domain;

namespace Minimal.Api.Features.Loans.Commands;

public class CreatePaymentLoanHandler : IRequestHandler<CreatePaymentLoan, LoanPaymentsGetDto>
{
    private readonly ApplicationDbContext _context;
    private readonly LoanMapper _mapper;
    private readonly IStringLocalizer _localizer;
    private readonly DocumentValidator _documentValidator;

    public CreatePaymentLoanHandler(
        ApplicationDbContext context,
        LoanMapper mapper,
        IStringLocalizer<SharedResource> localizer,
        DocumentValidator documentValidator)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
        _documentValidator = documentValidator ?? throw new ArgumentNullException(nameof(documentValidator));
    }

    public async Task<LoanPaymentsGetDto> Handle(CreatePaymentLoan request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var loan = await _context.Loans
            .Include(l => l.LoanType)
            .Include(l => l.AccountDetail)
            .FirstOrDefaultAsync(l => l.Id == request.LoanId, cancellationToken);
        if (loan is null)
        {
            throw new NotFoundException(_localizer.GetString("notFoundLoan").Value);
        }

        if (loan.IsActive is false)
        {
            throw new ErrorException(_localizer.GetString("loanIsNotActive").Value);
        }

        if (loan.CreateDate > request.PaymentDate)
        {
            throw new ValidationException(nameof(request.PaymentDate), _localizer.GetString("paymentDateIsEarlierLoanDate").Value);
        }

        var installments = await _context.LoanInstallments
            .Where(li => li.LoanId == request.LoanId)
            .OrderBy(li => li.Number)
            .ToListAsync(cancellationToken);

        if (installments.Sum(i => i.Amount) - installments.Sum(i => i.PaidAmount) < request.Amount)
        {
            throw new ValidationException(nameof(request.Amount), _localizer.GetString("paymentAmountBiggerThanRemaining").Value);
        }

        if (installments.Sum(i => i.Amount) - installments.Sum(i => i.PaidAmount) == request.Amount)
        {
            loan.CloseDate = request.PaymentDate;
            loan.IsActive = false;
        }

        var allocations = new List<LoanPaymentAllocation>();
        decimal remaining = request.Amount;

        foreach (var inst in installments)
        {
            if (remaining <= 0) break;

            decimal interestRemaining = inst.InterestAmount - inst.PaidInterest;
            decimal principalRemaining = inst.PrincipalAmount - inst.PaidPrincipal;

            if (interestRemaining <= 0 && principalRemaining <= 0)
            {
                continue;
            }

            var alloc = new LoanPaymentAllocation
            {
                InstallmentId = inst.Id,
                InterestAmount = 0,
                PrincipalAmount = 0
            };

            if (interestRemaining > 0)
            {
                var payInterest = Math.Min(remaining, interestRemaining);
                alloc.InterestAmount = payInterest;
                remaining -= payInterest;
            }

            if (remaining > 0 && principalRemaining > 0)
            {
                var payPrincipal = Math.Min(remaining, principalRemaining);
                alloc.PrincipalAmount = payPrincipal;
                remaining -= payPrincipal;
            }

            allocations.Add(alloc);
        }

        decimal totalPrincipal = allocations.Sum(a => a.PrincipalAmount);
        decimal totalInterest = allocations.Sum(a => a.InterestAmount);

        var fiscalYear = await _context.GetCurrentFiscalYearAsync(cancellationToken);
        var payInstallmentDocumentType = await _context.GetDocumentTypeByCodeAsync("22", cancellationToken);
        var bankAccountSubsid = await _context.GetBankAccountAsync(cancellationToken);
        var loanAccountSubsid = await _context.GetAccountSubsidByCodeAsync(loan.LoanType.Code, cancellationToken);
        var loanInterestIncomeSubsid = await _context.GetAccountSubsidByCodeAsync("3101", cancellationToken);

        var documentToAdd = new AccountingDocumentBuilder(fiscalYear, payInstallmentDocumentType, request.PaymentDate)
            .Debit(request.Amount, bankAccountSubsid);

        if (totalPrincipal > 0)
        {
            documentToAdd.Credit(totalPrincipal, loanAccountSubsid, loan.AccountDetail);
        }
        if (totalInterest > 0)
        {
            documentToAdd.Credit(totalInterest, loanInterestIncomeSubsid);
        }

        var loanDoc = documentToAdd.Build();
        var validation = _documentValidator.ValidateDocument(loanDoc);
        if (!validation.IsValid)
        {
            throw new ErrorException(validation.ErrorMessage);
        }

        _context.Documents.Add(loanDoc);

        var payment = new LoanPayment
        {
            LoanId = loan.Id,
            PaymentDate = request.PaymentDate,
            Amount = request.Amount,
            Note = request.Note,
            Document = loanDoc,
        };
        _context.LoanPayments.Add(payment);

        _context.LoanPaymentAllocations.AddRange(allocations.Select(a => new LoanPaymentAllocation
        {
            Payment = payment,
            InstallmentId = a.InstallmentId,
            PrincipalAmount = a.PrincipalAmount,
            InterestAmount = a.InterestAmount
        }).ToList());

        foreach (var alloc in allocations)
        {
            var inst = installments.First(x => x.Id == alloc.InstallmentId);

            inst.PaidPrincipal += alloc.PrincipalAmount;
            inst.PaidInterest += alloc.InterestAmount;

            if (inst.PaidPrincipal == inst.PrincipalAmount && inst.PaidInterest == inst.InterestAmount)
            {
                inst.Status = InstallmentStatus.Paid;
                inst.PaidDate = request.PaymentDate;
            }
            else if (inst.PaidPrincipal > 0 || inst.PaidInterest > 0)
            {
                inst.Status = InstallmentStatus.PartiallyPaid;
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.MapToLoanPaymentGetDto(payment);
    }
}