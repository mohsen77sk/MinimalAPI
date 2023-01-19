using FluentValidation;

namespace Minimal.Api.Features.People.Commands;

public class UpdateStatusPersonValidator : AbstractValidator<UpdateStatusPerson>
{
    public UpdateStatusPersonValidator()
    {
        RuleFor(r => r.Id).NotEmpty();
        RuleFor(r => r.IsActive).NotNull();
    }
}