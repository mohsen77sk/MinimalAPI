using FluentValidation;

namespace Minimal.Api.Features.LoanTransactions.Commands;

public class UpdateLoanTransactionValidator : AbstractValidator<UpdateLoanTransaction>
{
    public UpdateLoanTransactionValidator()
    {
        RuleFor(r => r.Id).NotEmpty();
        RuleFor(r => r.LoanId).NotEmpty();
        RuleFor(r => r.Amount).GreaterThan(0);
        RuleFor(r => r.Date).LessThanOrEqualTo(DateTimeOffset.Now);
    }
}