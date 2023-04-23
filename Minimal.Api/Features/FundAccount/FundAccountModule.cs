using MediatR;
using Minimal.Api.Contracts;
using Minimal.Api.Features.FundAccount.Models;
using Minimal.Api.Features.FundAccount.Queries;
using Minimal.Api.Models;

namespace Minimal.Api.Features.FundAccount;

public class BankModule : IModule
{
    public IEndpointRouteBuilder RegisterEndpoints(IEndpointRouteBuilder endpoints)
    {
        var fund = endpoints.MapGroup("/api/fund").WithDisplayName("Fund").RequireAuthorization();

        fund.MapGet("/balance", GetFundAccountBalanceAsync)
            .Produces<FundAccountBalanceGetDto>()
            .Produces(404)
            .Produces(500);

        fund.MapGet("/Transactions", GetFundAccountTransactionsAsync)
            .Produces<PageList<FundAccountTransactionGetDto>>()
            .Produces(404)
            .Produces(500);

        return endpoints;
    }

    private async Task<IResult> GetFundAccountBalanceAsync(IMediator mediator, CancellationToken ct)
    {
        var query = new GetFundAccountBalance();
        var account = await mediator.Send(query, ct);
        return Results.Ok(account);
    }

    private async Task<IResult> GetFundAccountTransactionsAsync([AsParameters] GetFundAccountTransactions request, IMediator mediator, CancellationToken ct)
    {
        var accountTransactions = await mediator.Send(request, ct);
        return Results.Ok(accountTransactions);
    }
}