using FluentValidation;

namespace Minimal.Api.Features.LoanTransactions.Commands;

public class CreateLoanTransactionValidator : AbstractValidator<CreateLoanTransaction>
{
    public CreateLoanTransactionValidator()
    {
        RuleFor(r => r.LoanId).NotEmpty();
        RuleFor(r => r.Amount).GreaterThan(0);
        RuleFor(r => r.Date).LessThanOrEqualTo(DateTimeOffset.Now);
    }
}