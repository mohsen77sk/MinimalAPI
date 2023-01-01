using MediatR;
using Minimal.Api.Models;

namespace Minimal.Api.Features.AccountTypes.Queries;

public class GetLookupAccountType : IRequest<List<LookupDto>>
{
}