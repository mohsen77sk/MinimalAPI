using MediatR;
using Minimal.Api.Features.People.Models;

namespace Minimal.Api.Features.People.Commands;

public class CreatePerson : IRequest<PersonGetDto>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string NationalCode { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public GenderEnum Gender { get; set; }
    public string Note { get; set; } = string.Empty;
}