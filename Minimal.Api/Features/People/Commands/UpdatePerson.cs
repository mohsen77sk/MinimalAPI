using MediatR;
using Minimal.Api.Features.People.Models;

namespace Minimal.Api.Features.People.Commands;

public class UpdatePerson : IRequest<PersonGetDto>
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string NationalCode { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public byte Gender { get; set; }
    public string Note { get; set; } = string.Empty;
}