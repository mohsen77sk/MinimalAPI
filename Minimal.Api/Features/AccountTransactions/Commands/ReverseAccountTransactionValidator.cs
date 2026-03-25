using FluentValidation;

namespace Minimal.Api.Features.AccountTransactions.Commands;

public class ReverseAccountTransactionValidator : AbstractValidator<ReverseAccountTransaction>
{
    public ReverseAccountTransactionValidator()
    {
        RuleFor(r => r.Id).NotEmpty();
        RuleFor(r => r.AccountId).NotEmpty();
    }
}