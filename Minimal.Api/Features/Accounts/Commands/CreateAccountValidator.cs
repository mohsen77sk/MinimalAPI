using FluentValidation;

namespace Minimal.Api.Features.Accounts.Commands;

public class CreateAccountValidator : AbstractValidator<CreateAccount>
{
    public CreateAccountValidator()
    {
        RuleFor(r => r.AccountTypeId).NotEmpty();
        RuleFor(r => r.InitCredit).GreaterThanOrEqualTo(1000);
        RuleFor(r => r.CreateDate).LessThan(DateTimeOffset.Now);
        RuleFor(r => r.PersonId).NotEmpty();
    }
}