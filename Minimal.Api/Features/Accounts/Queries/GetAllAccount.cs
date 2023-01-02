using MediatR;
using Minimal.Api.Features.Accounts.Models;
using Minimal.Api.Models;

namespace Minimal.Api.Features.Accounts.Queries;

public class GetAllAccount : PagingData, IRequest<PageList<AccountGetDto>>
{
}