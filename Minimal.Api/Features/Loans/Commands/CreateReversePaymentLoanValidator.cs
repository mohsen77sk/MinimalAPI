using FluentValidation;

namespace Minimal.Api.Features.Loans.Commands;

public class CreateReversePaymentLoanValidator : AbstractValidator<CreateReversePaymentLoan>
{
    public CreateReversePaymentLoanValidator()
    {
        RuleFor(r => r.Id).NotEmpty();
        RuleFor(r => r.LoanId).NotEmpty();
    }
}