using MediatR;
using Minimal.Api.Contracts;
using Minimal.Api.Features.Accounts.Commands;
using Minimal.Api.Features.Accounts.Models;
using Minimal.Api.Features.Accounts.Queries;
using Minimal.Api.Models;

namespace Minimal.Api.Features.Accounts;

public class AccountModule : IModule
{
    public IEndpointRouteBuilder RegisterEndpoints(IEndpointRouteBuilder endpoints)
    {
        var accounts = endpoints.MapGroup("/api/account").WithDisplayName("Accounts").RequireAuthorization();

        accounts.MapGet("/all", GetAccountsAsync)
            .Produces<PageList<AccountGetDto>>()
            .Produces(500);

        accounts.MapGet("/lookup", GetLookupAccountsAsync)
            .Produces<List<LookupDto>>()
            .Produces(500);

        accounts.MapPost("/", CreateAccountAsync)
            .Produces<AccountGetDto>()
            .Produces<ValidationError>(400)
            .Produces(500);

        accounts.MapPut("/", UpdateAccountAsync)
            .Produces<AccountGetDto>()
            .Produces<ValidationError>(400)
            .Produces(404)
            .Produces(500);

        accounts.MapGet("/{id}", GetAccountByIdAsync)
            .Produces<AccountGetDto>()
            .Produces(404)
            .Produces(500);

        accounts.MapGet("/{id}/balance", GetAccountBalanceAsync)
            .Produces<AccountBalanceGetDto>()
            .Produces(404)
            .Produces(500);

        accounts.MapPost("/close", CloseAccountAsync)
            .Produces<AccountGetDto>()
            .Produces<ValidationError>(400)
            .Produces(500);

        return endpoints;
    }

    private async Task<IResult> GetAccountsAsync([AsParameters] GetAllAccount request, IMediator mediator, CancellationToken ct)
    {
        var accounts = await mediator.Send(request, ct);
        return Results.Ok(accounts);
    }

    /// <remarks>
    /// ### * Get active accounts
    /// </remarks>
    private async Task<IResult> GetLookupAccountsAsync(IMediator mediator, CancellationToken ct)
    {
        var query = new GetLookupAccounts();
        var accounts = await mediator.Send(query, ct);
        return Results.Ok(accounts);
    }

    private async Task<IResult> CreateAccountAsync(CreateAccount accountDto, IMediator mediator, CancellationToken ct)
    {
        var account = await mediator.Send(accountDto, ct);
        return Results.Ok(account);
    }

    private async Task<IResult> UpdateAccountAsync(UpdateAccount accountDto, IMediator mediator, CancellationToken ct)
    {
        var account = await mediator.Send(accountDto, ct);
        return Results.Ok(account);
    }

    private async Task<IResult> GetAccountByIdAsync(int id, IMediator mediator, CancellationToken ct)
    {
        var query = new GetAccountById { AccountId = id };
        var account = await mediator.Send(query, ct);
        return Results.Ok(account);
    }

    private async Task<IResult> GetAccountBalanceAsync(int id, IMediator mediator, CancellationToken ct)
    {
        var query = new GetAccountBalance { AccountId = id };
        var account = await mediator.Send(query, ct);
        return Results.Ok(account);
    }

    private async Task<IResult> CloseAccountAsync(CloseAccount accountDto, IMediator mediator, CancellationToken ct)
    {
        var account = await mediator.Send(accountDto, ct);
        return Results.Ok(account);
    }
}