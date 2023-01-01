using MediatR;
using Minimal.Api.Models;

namespace Minimal.Api.Features.Banks.Queries;

public class GetLookupBank : IRequest<List<LookupDto>>
{
}