using MediatR;
using Minimal.Api.Contracts;
using Minimal.Api.Features.AccountTransactions.Commands;
using Minimal.Api.Features.AccountTransactions.Models;
using Minimal.Api.Features.AccountTransactions.Queries;
using Minimal.Api.Models;

namespace Minimal.Api.Features.AccountTransactions;

public class AccountTransactionModule : IModule
{
    public IEndpointRouteBuilder RegisterEndpoints(IEndpointRouteBuilder endpoints)
    {
        var transactions = endpoints.MapGroup("/api/accountTransaction").WithDisplayName("AccountTransactions").RequireAuthorization();

        transactions.MapGet("/all", GetTransactionAccountsAsync)
            .Produces<PageList<AccountTransactionGetDto>>()
            .Produces(500);

        transactions.MapPost("/", CreateTransactionAccountAsync)
            .Produces<AccountTransactionGetDto>()
            .Produces<ValidationError>(400)
            .Produces(500);

        transactions.MapPut("/", UpdateTransactionAccountAsync)
            .Produces<AccountTransactionGetDto>()
            .Produces<ValidationError>(400)
            .Produces(500);

        transactions.MapPost("/reverse", ReverseTransactionAccountAsync)
            .Produces<AccountTransactionGetDto>()
            .Produces<ValidationError>(400)
            .Produces(500);

        return endpoints;
    }

    private async Task<IResult> GetTransactionAccountsAsync([AsParameters] GetAllAccountTransaction request, IMediator mediator, CancellationToken ct)
    {
        var transactions = await mediator.Send(request, ct);
        return Results.Ok(transactions);
    }

    private async Task<IResult> CreateTransactionAccountAsync(CreateAccountTransaction accountTransactionDto, IMediator mediator, CancellationToken ct)
    {
        var transaction = await mediator.Send(accountTransactionDto, ct);
        return Results.Ok(transaction);
    }

    private async Task<IResult> UpdateTransactionAccountAsync(UpdateAccountTransaction accountTransactionDto, IMediator mediator, CancellationToken ct)
    {
        var transaction = await mediator.Send(accountTransactionDto, ct);
        return Results.Ok(transaction);
    }

    private async Task<IResult> ReverseTransactionAccountAsync(ReverseAccountTransaction accountTransactionDto, IMediator mediator, CancellationToken ct)
    {
        var transaction = await mediator.Send(accountTransactionDto, ct);
        return Results.Ok(transaction);
    }
}