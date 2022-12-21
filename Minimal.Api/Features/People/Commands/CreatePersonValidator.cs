using FluentValidation;

namespace Minimal.Api.Features.People.Commands;

public class CreatePersonValidator : AbstractValidator<CreatePerson>
{
    public CreatePersonValidator()
    {
        RuleFor(r => r.FirstName).NotEmpty().Length(2, 25);
        RuleFor(r => r.LastName).NotEmpty().Length(2, 25);
        RuleFor(r => r.NationalCode).Must(x => x.Length == 0 || x.Length == 10);
    }
}