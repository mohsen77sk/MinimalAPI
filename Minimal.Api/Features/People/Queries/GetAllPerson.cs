using MediatR;
using Minimal.Api.Features.People.Models;

namespace Minimal.Api.Features.People.Queries;

public class GetAllPerson : IRequest<ICollection<PeopleGetDto>>
{
}