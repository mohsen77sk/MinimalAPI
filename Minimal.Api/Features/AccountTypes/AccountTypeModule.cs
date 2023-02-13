using MediatR;
using Minimal.Api.Contracts;
using Minimal.Api.Features.AccountTypes.Models;
using Minimal.Api.Features.AccountTypes.Queries;
using Minimal.Api.Models;

namespace Minimal.Api.Features.AccountTypes;

public class AccountTypeModule : IModule
{
    public IEndpointRouteBuilder RegisterEndpoints(IEndpointRouteBuilder endpoints)
    {
        var accountTypes = endpoints.MapGroup("/api/accountType").WithDisplayName("AccountTypes").RequireAuthorization();

        accountTypes.MapGet("/all", GetAllAccountTypesAsync)
            .Produces<PageList<AccountTypeGetDto>>()
            .Produces(500);

        accountTypes.MapGet("/lookup", GetLookupAccountTypesAsync)
            .Produces<List<LookupDto>>()
            .Produces(500);

        return endpoints;
    }

    private async Task<IResult> GetAllAccountTypesAsync([AsParameters] GetAllAccountType request, IMediator mediator, CancellationToken ct)
    {
        var accountTypes = await mediator.Send(request, ct);
        return Results.Ok(accountTypes);
    }

    /// <remarks>
    /// ### * Get active account types
    /// </remarks>
    private async Task<IResult> GetLookupAccountTypesAsync(IMediator mediator, CancellationToken ct)
    {
        var query = new GetLookupAccountType();
        var accountTypes = await mediator.Send(query, ct);
        return Results.Ok(accountTypes);
    }
}