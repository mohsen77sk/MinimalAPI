using MediatR;
using Minimal.Api.Features.AccountTypes.Models;
using Minimal.Api.Models;

namespace Minimal.Api.Features.AccountTypes.Queries;

public class GetAllAccountType : PagingData, IRequest<PageList<AccountTypeGetDto>>
{
    public string? Search { get; set; }
    public bool? IsActive { get; set; }
}