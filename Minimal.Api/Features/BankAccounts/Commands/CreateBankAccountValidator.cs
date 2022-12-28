using FluentValidation;

namespace Minimal.Api.Features.BankAccounts.Commands;

public class CreateBankAccountValidator : AbstractValidator<CreateBankAccount>
{
    public CreateBankAccountValidator()
    {
        RuleFor(r => r.BankId).NotEmpty();
        RuleFor(r => r.PersonId).NotEmpty();
        RuleFor(r => r.BranchCode).Length(2, 25);
        RuleFor(r => r.BranchName).Length(2, 25);
        RuleFor(r => r.AccountNumber)
            .Length(10, 26).When(s => !string.IsNullOrEmpty(s.AccountNumber));
        RuleFor(r => r.CardNumber)
            .Length(16).When(s => !string.IsNullOrEmpty(s.CardNumber));
        RuleFor(r => r.Iban)
            .Length(26).When(s => !string.IsNullOrEmpty(s.Iban));
    }
}