using MediatR;
using Minimal.Api.Features.People.Models;

namespace Minimal.Api.Features.People.Queries;

public class GetPersonById : IRequest<PersonGetDto>
{
    public int PersonId { get; set; }
}