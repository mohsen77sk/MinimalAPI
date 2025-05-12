using FluentValidation;

namespace Minimal.Api.Features.AccountTransactions.Commands;

public class CreateAccountTransactionValidator : AbstractValidator<CreateAccountTransaction>
{
    public CreateAccountTransactionValidator()
    {
        RuleFor(r => r.SourceAccountId).NotEmpty();
        RuleFor(r => r.DestinationAccountId).NotEmpty().When(s => s.TransactionType == Models.TransactionTypeEnum.Transfer);
        RuleFor(r => r.TransactionType).IsInEnum();
        RuleFor(r => r.Amount).GreaterThan(0);
        RuleFor(r => r.Date).LessThanOrEqualTo(DateTimeOffset.Now);
    }
}