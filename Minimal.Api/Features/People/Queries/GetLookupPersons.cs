using MediatR;
using Minimal.Api.Models;

namespace Minimal.Api.Features.People.Queries;

public class GetLookupPersons : IRequest<List<LookupDto>>
{
}