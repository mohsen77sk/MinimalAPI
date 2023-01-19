using MediatR;
using Minimal.Api.Features.People.Models;

namespace Minimal.Api.Features.People.Commands;

public class UpdateStatusPerson : IRequest<PersonGetDto>
{
    public int Id { get; set; }
    public bool IsActive { get; set; }
}