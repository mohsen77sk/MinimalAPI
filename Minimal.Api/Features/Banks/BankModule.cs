
using MediatR;
using Minimal.Api.Contracts;
using Minimal.Api.Features.Banks.Models;
using Minimal.Api.Features.Banks.Queries;
using Minimal.Api.Models;

namespace Minimal.Api.Features.Banks;

public class BankModule : IModule
{
    public IEndpointRouteBuilder RegisterEndpoints(IEndpointRouteBuilder endpoints)
    {
        var banks = endpoints.MapGroup("/api/bank").WithDisplayName("Banks").RequireAuthorization();

        banks.MapGet("/all", GetAllBanksAsync)
            .Produces<PageList<BankGetDto>>()
            .Produces(500);

        banks.MapGet("/lookup", GetLookupBanksAsync)
            .Produces<List<LookupDto>>()
            .Produces(500);

        return endpoints;
    }

    private async Task<IResult> GetAllBanksAsync([AsParameters] PagingData request, IMediator mediator, CancellationToken ct)
    {
        var query = new GetAllBank
        {
            Page = request.Page,
            PageSize = request.PageSize,
            SortBy = request.SortBy
        };
        var banks = await mediator.Send(query, ct);
        return Results.Ok(banks);
    }

    private async Task<IResult> GetLookupBanksAsync(IMediator mediator, CancellationToken ct)
    {
        var query = new GetLookupBank();
        var banks = await mediator.Send(query, ct);
        return Results.Ok(banks);
    }
}
