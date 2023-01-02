using FluentValidation;

namespace Minimal.Api.Features.Accounts.Commands;

public class UpdateAccountValidator : AbstractValidator<UpdateAccount>
{
    public UpdateAccountValidator()
    {
        RuleFor(r => r.Id).NotEmpty();
        RuleFor(r => r.AccountTypeId).NotEmpty();
        RuleFor(r => r.InitCredit).GreaterThanOrEqualTo(1000);
        RuleFor(r => r.CreateDate).LessThanOrEqualTo(DateTimeOffset.Now);
        RuleFor(r => r.PersonId).NotEmpty();
    }
}