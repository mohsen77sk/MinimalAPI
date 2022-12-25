using FluentValidation;

namespace Minimal.Api.Features.People.Commands;

public class CreatePersonValidator : AbstractValidator<CreatePerson>
{
    public CreatePersonValidator()
    {
        RuleFor(r => r.FirstName).NotEmpty().Length(2, 25);
        RuleFor(r => r.LastName).NotEmpty().Length(2, 25);
        RuleFor(r => r.NationalCode).Length(10).When(s => !string.IsNullOrEmpty(s.NationalCode));
    }
}