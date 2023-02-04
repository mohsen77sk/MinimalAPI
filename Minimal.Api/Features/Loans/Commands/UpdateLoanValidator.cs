using FluentValidation;

namespace Minimal.Api.Features.Loans.Commands;

public class UpdateLoanValidator : AbstractValidator<UpdateLoan>
{
    public UpdateLoanValidator()
    {
        RuleFor(r => r.AccountId).NotEmpty();
    }
}