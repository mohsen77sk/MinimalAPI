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

    private async Task<IResult> GetAllAccountTypesAsync([AsParameters] PagingData request, IMediator mediator, CancellationToken ct)
    {
        var query = new GetAllAccountType
        {
            Page = request.Page,
            PageSize = request.PageSize,
            SortBy = request.SortBy
        };
        var accountTypes = await mediator.Send(query, ct);
        return Results.Ok(accountTypes);
    }

    private async Task<IResult> GetLookupAccountTypesAsync(IMediator mediator, CancellationToken ct)
    {
        var query = new GetLookupAccountType();
        var accountTypes = await mediator.Send(query, ct);
        return Results.Ok(accountTypes);
    }
}