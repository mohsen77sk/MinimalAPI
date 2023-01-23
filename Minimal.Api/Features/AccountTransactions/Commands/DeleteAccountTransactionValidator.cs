using FluentValidation;

namespace Minimal.Api.Features.AccountTransactions.Commands;

public class DeleteAccountTransactionValidator : AbstractValidator<DeleteAccountTransaction>
{
    public DeleteAccountTransactionValidator()
    {
        RuleFor(r => r.Id).NotEmpty();
    }
}