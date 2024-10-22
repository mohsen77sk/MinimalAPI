using MediatR;
using Minimal.Api.Models;

namespace Minimal.Api.Features.Accounts.Queries;

public class GetLookupAccounts : IRequest<List<LookupDto>>
{
}