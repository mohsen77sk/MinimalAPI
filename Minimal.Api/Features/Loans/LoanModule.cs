using MediatR;
using Minimal.Api.Contracts;
using Minimal.Api.Features.Loans.Commands;
using Minimal.Api.Features.Loans.Models;
using Minimal.Api.Features.Loans.Queries;
using Minimal.Api.Models;

namespace Minimal.Api.Features.Loans;

public class LoanModule : IModule
{
    public IEndpointRouteBuilder RegisterEndpoints(IEndpointRouteBuilder endpoints)
    {
        var loans = endpoints.MapGroup("/api/loan").WithDisplayName("Loans").RequireAuthorization();

        loans.MapGet("/all", GetLoansAsync)
            .Produces<PageList<LoanGetDto>>()
            .Produces(500);

        loans.MapPost("/", CreateLoanAsync)
            .Produces<LoanGetDto>()
            .Produces<ValidationError>(400)
            .Produces(500);

        loans.MapPut("/", UpdateLoanAsync)
            .Produces<LoanGetDto>()
            .Produces<ValidationError>(400)
            .Produces(404)
            .Produces(500);

        loans.MapGet("/{id}", GetLoanByIdAsync)
            .Produces<LoanGetDto>()
            .Produces(404)
            .Produces(500);

        return endpoints;
    }

    private async Task<IResult> GetLoansAsync([AsParameters] GetAllLoan request, IMediator mediator, CancellationToken ct)
    {
        var loans = await mediator.Send(request, ct);
        return Results.Ok(loans);
    }

    private async Task<IResult> CreateLoanAsync(CreateLoan loanDto, IMediator mediator, CancellationToken ct)
    {
        var loan = await mediator.Send(loanDto, ct);
        return Results.Ok(loan);
    }

    private async Task<IResult> UpdateLoanAsync(UpdateLoan loanDto, IMediator mediator, CancellationToken ct)
    {
        var loan = await mediator.Send(loanDto, ct);
        return Results.Ok(loan);
    }

    private async Task<IResult> GetLoanByIdAsync(int id, IMediator mediator, CancellationToken ct)
    {
        var query = new GetLoanById { LoanId = id };
        var loan = await mediator.Send(query, ct);
        return Results.Ok(loan);
    }
}