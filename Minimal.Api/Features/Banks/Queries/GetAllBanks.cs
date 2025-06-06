using MediatR;
using Minimal.Api.Features.Banks.Models;
using Minimal.Api.Models;

namespace Minimal.Api.Features.Banks.Queries;

public class GetAllBank : PagingData, IRequest<PageList<BankGetDto>>
{
    public string? Search { get; set; }
    public bool? IsActive { get; set; }
}