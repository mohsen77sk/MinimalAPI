using FluentValidation;

namespace Minimal.Api.Features.AccountTransactions.Commands;

public class UpdateAccountTransactionValidator : AbstractValidator<UpdateAccountTransaction>
{
    public UpdateAccountTransactionValidator()
    {
        RuleFor(r => r.Id).NotEmpty();
        RuleFor(r => r.AccountId).NotEmpty();
        RuleFor(r => r.Amount).GreaterThan(0);
        RuleFor(r => r.TransactionType).IsInEnum();
    }
}