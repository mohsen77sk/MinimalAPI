using FluentValidation;

namespace Minimal.Api.Features.AccountTransactions.Commands;

public class CreateAccountTransactionValidator : AbstractValidator<CreateAccountTransaction>
{
    public CreateAccountTransactionValidator()
    {
        RuleFor(r => r.AccountId).NotEmpty();
        RuleFor(r => r.Amount).GreaterThan(0);
        RuleFor(r => r.TransactionType).IsInEnum();
        RuleFor(r => r.Date).LessThanOrEqualTo(DateTimeOffset.Now);
    }
}