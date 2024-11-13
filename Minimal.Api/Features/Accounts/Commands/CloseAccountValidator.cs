using FluentValidation;

namespace Minimal.Api.Features.Accounts.Commands;

public class CloseAccountValidator : AbstractValidator<CloseAccount>
{
    public CloseAccountValidator()
    {
        RuleFor(r => r.Id).NotEmpty();
        RuleFor(r => r.CloseDate).LessThan(DateTimeOffset.Now);
    }
}