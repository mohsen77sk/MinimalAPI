using FluentValidation;

namespace Minimal.Api.Features.BankAccounts.Commands;

public class UpdateStatusBankAccountValidator : AbstractValidator<UpdateStatusBankAccount>
{
    public UpdateStatusBankAccountValidator()
    {
        RuleFor(r => r.Id).NotEmpty();
        RuleFor(r => r.IsActive).NotNull();
    }
}