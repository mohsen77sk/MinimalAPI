using FluentValidation;

namespace Minimal.Api.Features.BankAccounts.Commands;

public class DeleteBankAccountValidator : AbstractValidator<DeleteBankAccount>
{
    public DeleteBankAccountValidator()
    {
        RuleFor(r => r.Id).NotEmpty();
    }
}