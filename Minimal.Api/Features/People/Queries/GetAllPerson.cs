using MediatR;
using Minimal.Api.Features.People.Models;
using Minimal.Api.Models;

namespace Minimal.Api.Features.People.Queries;

public class GetAllPerson : PagingData, IRequest<PageList<PersonGetDto>>
{
    public string? Search { get; set; }
    public bool? IsActive { get; set; }
}