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

    private async Task<IResult> GetAllLoanTypesAsync([AsParameters] GetAllLoanType request, IMediator mediator, CancellationToken ct)
    {
        var loanTypes = await mediator.Send(request, ct);
        return Results.Ok(loanTypes);
    }

    /// <remarks>
    /// ### * Get active loan types
    /// </remarks>
    private async Task<IResult> GetLookupLoanTypesAsync(IMediator mediator, CancellationToken ct)
    {
        var query = new GetLookupLoanType();
        var loanTypes = await mediator.Send(query, ct);
        return Results.Ok(loanTypes);
    }
}