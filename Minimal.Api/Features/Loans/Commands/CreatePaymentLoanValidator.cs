using FluentValidation;

namespace Minimal.Api.Features.Loans.Commands;

public class CreatePaymentLoanValidator : AbstractValidator<CreatePaymentLoan>
{
    public CreatePaymentLoanValidator()
    {
        RuleFor(r => r.LoanId).NotEmpty();
        RuleFor(r => r.PaymentDate).LessThanOrEqualTo(DateTimeOffset.Now);
        RuleFor(r => r.Amount).GreaterThan(0);
    }
}