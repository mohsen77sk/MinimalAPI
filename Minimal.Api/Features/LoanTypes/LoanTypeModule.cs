using MediatR;
using Minimal.Api.Contracts;
using Minimal.Api.Features.LoanTypes.Models;
using Minimal.Api.Features.LoanTypes.Queries;
using Minimal.Api.Models;

namespace Minimal.Api.Features.LoanTypes;

public class LoanTypeModule : IModule
{
    public IEndpointRouteBuilder RegisterEndpoints(IEndpointRouteBuilder endpoints)
    {
        var loanTypes = endpoints.MapGroup("/api/loanType").WithDisplayName("LoanTypes").RequireAuthorization();

        loanTypes.MapGet("/all", GetAllLoanTypesAsync)
            .Produces<PageList<LoanTypeGetDto>>()
            .Produces(500);

        loanTypes.MapGet("/lookup", GetLookupLoanTypesAsync)
            .Produces<List<LookupDto>>()
            .Produces(500);

        return endpoints;
    }

    private async Task<IResult> GetAllLoanTypesAsync([AsParameters] PagingData request, IMediator mediator, CancellationToken ct)
    {
        var query = new GetAllLoanType
        {
            Page = request.Page,
            PageSize = request.PageSize,
            SortBy = request.SortBy
        };
        var loanTypes = await mediator.Send(query, ct);
        return Results.Ok(loanTypes);
    }

    private async Task<IResult> GetLookupLoanTypesAsync(IMediator mediator, CancellationToken ct)
    {
        var query = new GetLookupLoanType();
        var loanTypes = await mediator.Send(query, ct);
        return Results.Ok(loanTypes);
    }
}