using MediatR;
using Minimal.Api.Contracts;
using Minimal.Api.Features.AccountTypes.Models;
using Minimal.Api.Features.AccountTypes.Queries;

namespace Minimal.Api.Features.AccountTypes;

public class AccountTypeModule : IModule
{
    public IEndpointRouteBuilder RegisterEndpoints(IEndpointRouteBuilder endpoints)
    {
        var accountTypes = endpoints.MapGroup("/api/accountType").WithDisplayName("AccountTypes").RequireAuthorization();

        accountTypes.MapGet("/all", GetAllAccountTypesAsync)
            .Produces<List<AccountTypeGetDto>>()
            .Produces(500);

        return endpoints;
    }

    private async Task<IResult> GetAllAccountTypesAsync(IMediator mediator, CancellationToken ct)
    {
        var query = new GetAllAccountType();
        var accountTypes = await mediator.Send(query, ct);
        return Results.Ok(accountTypes);
    }
}