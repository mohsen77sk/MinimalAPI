
using MediatR;
using Minimal.Api.Contracts;
using Minimal.Api.Features.Banks.Models;
using Minimal.Api.Features.Banks.Queries;

namespace Minimal.Api.Features.Banks;

public class BankModule : IModule
{
    public IEndpointRouteBuilder RegisterEndpoints(IEndpointRouteBuilder endpoints)
    {
        var banks = endpoints.MapGroup("/api/bank").WithDisplayName("Banks").RequireAuthorization();

        banks.MapGet("/all", GetAllBanksAsync)
            .Produces<List<BankGetDto>>()
            .Produces(500);

        return endpoints;
    }

    private async Task<IResult> GetAllBanksAsync(IMediator mediator, CancellationToken ct)
    {
        var query = new GetAllBank();
        var banks = await mediator.Send(query, ct);
        return Results.Ok(banks);
    }
}
