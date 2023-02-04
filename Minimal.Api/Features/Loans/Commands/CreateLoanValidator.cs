using FluentValidation;

namespace Minimal.Api.Features.Loans.Commands;

public class CreateLoanValidator : AbstractValidator<CreateLoan>
{
    public CreateLoanValidator()
    {
        RuleFor(r => r.AccountId).NotEmpty();
        RuleFor(r => r.LoanTypeId).NotEmpty();
        RuleFor(r => r.CreateDate).LessThanOrEqualTo(DateTimeOffset.Now);
        RuleFor(r => r.Amount).GreaterThan(0);
        RuleFor(r => r.InstallmentCount).InclusiveBetween(1, 50);
        RuleFor(r => r.InstallmentInterval).InclusiveBetween(1, 12);
        RuleFor(r => r.InterestRates).InclusiveBetween(1, 100);
    }
}