using FluentValidation;

namespace Minimal.Api.Features.Accounts.Commands;

public class UpdateAccountValidator : AbstractValidator<UpdateAccount>
{
    public UpdateAccountValidator()
    {
        RuleFor(r => r.Id).NotEmpty();
        RuleFor(r => r.PersonId).NotEmpty();
    }
}