using MediatR;
using Minimal.Api.Contracts;
using Minimal.Api.Features.LoanTransactions.Commands;
using Minimal.Api.Features.LoanTransactions.Models;
using Minimal.Api.Features.LoanTransactions.Queries;
using Minimal.Api.Models;

namespace Minimal.Api.Features.LoanTransactions;

public class LoanTransactionModule : IModule
{
    public IEndpointRouteBuilder RegisterEndpoints(IEndpointRouteBuilder endpoints)
    {
        var transactions = endpoints.MapGroup("/api/loanTransaction").WithDisplayName("LoanTransactions").RequireAuthorization();

        transactions.MapGet("/all", GetTransactionLoansAsync)
            .Produces<PageList<LoanTransactionGetDto>>()
            .Produces(500);

        transactions.MapPost("/", CreateTransactionLoanAsync)
            .Produces<LoanTransactionGetDto>()
            .Produces<ValidationError>(400)
            .Produces(500);

        return endpoints;
    }

    private async Task<IResult> GetTransactionLoansAsync([AsParameters] GetAllLoanTransaction request, IMediator mediator, CancellationToken ct)
    {
        var loans = await mediator.Send(request, ct);
        return Results.Ok(loans);
    }

    private async Task<IResult> CreateTransactionLoanAsync(CreateLoanTransaction LoanTransactionDto, IMediator mediator, CancellationToken ct)
    {
        var loan = await mediator.Send(LoanTransactionDto, ct);
        return Results.Ok(loan);
    }
}