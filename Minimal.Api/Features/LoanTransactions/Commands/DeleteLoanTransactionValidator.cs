using FluentValidation;

namespace Minimal.Api.Features.LoanTransactions.Commands;

public class DeleteLoanTransactionValidator : AbstractValidator<DeleteLoanTransaction>
{
    public DeleteLoanTransactionValidator()
    {
        RuleFor(r => r.Id).NotEmpty();
    }
}