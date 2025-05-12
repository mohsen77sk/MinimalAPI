using FluentValidation;

namespace Minimal.Api.Features.AccountTransactions.Commands;

public class UpdateAccountTransactionValidator : AbstractValidator<UpdateAccountTransaction>
{
    public UpdateAccountTransactionValidator()
    {
        RuleFor(r => r.Id).NotEmpty();
    }
}