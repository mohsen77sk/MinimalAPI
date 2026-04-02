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

public class CreateReversePaymentLoanHandler : IRequestHandler<CreateReversePaymentLoan, LoanPaymentsGetDto>
{
    private readonly ApplicationDbContext _context;
    private readonly LoanMapper _mapper;
    private readonly IStringLocalizer _localizer;
    private readonly DocumentValidator _documentValidator;

    public CreateReversePaymentLoanHandler(
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

    public async Task<LoanPaymentsGetDto> Handle(CreateReversePaymentLoan request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var loan = await _context.Loans
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

        var payment = await _context.LoanPayments
            .Include(x => x.Document)
            .ThenInclude(x => x.DocumentItems)
            .ThenInclude(x => x.AccountDetail)
            .Include(x => x.Allocations)
            .ThenInclude(a => a.Installment)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (payment is null)
        {
            throw new NotFoundException(_localizer.GetString("notFoundPayment").Value);
        }

        if (payment.Document.Status == DocumentStatusEnum.Reversed)
        {
            throw new ErrorException(_localizer.GetString("paymentIsReversed").Value);
        }

        if (payment.Document.DocumentItems.Any(x => x.AccountDetail?.IsActive == false))
        {
            throw new ErrorException(_localizer.GetString("paymentCanNotBeReversed").Value);
        }

        foreach (var alloc in payment.Allocations)
        {
            var inst = alloc.Installment;

            inst.PaidPrincipal -= alloc.PrincipalAmount;
            inst.PaidInterest -= alloc.InterestAmount;

            if (inst.PaidPrincipal < 0 || inst.PaidInterest < 0)
            {
                throw new ErrorException(_localizer.GetString("InvalidReverseOperation").Value);
            }

            if (inst.PaidPrincipal == 0 && inst.PaidInterest == 0)
            {
                inst.Status = InstallmentStatus.Pending;
                inst.PaidDate = null;
            }
            else
            {
                inst.Status = InstallmentStatus.PartiallyPaid;
                inst.PaidDate = null;
            }
        }

        var reversalDocumentType = await _context.GetDocumentTypeByCodeAsync("15", cancellationToken);

        var reverseDocument = new AccountingDocumentReversalBuilder(payment.Document, reversalDocumentType, request.Note).Build();

        var validation = _documentValidator.ValidateDocument(reverseDocument);
        if (!validation.IsValid)
        {
            throw new ErrorException(validation.ErrorMessage);
        }

        _context.Documents.Add(reverseDocument);

        payment.Document.Status = DocumentStatusEnum.Reversed;

        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.MapToLoanPaymentGetDto(payment);
    }
}