using MediatR;
using Minimal.Api.Features.People.Models;
using Minimal.Api.Models;

namespace Minimal.Api.Features.People.Queries;

public class GetAllPerson : PagingData, IRequest<PageList<PeopleGetDto>>
{
}