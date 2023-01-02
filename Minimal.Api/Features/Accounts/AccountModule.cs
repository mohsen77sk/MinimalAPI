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

        accounts.MapGet("/{id}", GetAccountByIdAsync)
            .Produces<AccountGetDto>()
            .Produces(404)
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

        return endpoints;
    }

    private async Task<IResult> GetAccountsAsync([AsParameters] PagingData request, IMediator mediator, CancellationToken ct)
    {
        var query = new GetAllAccount
        {
            Page = request.Page,
            PageSize = request.PageSize,
            SortBy = request.SortBy
        };
        var accounts = await mediator.Send(query, ct);
        return Results.Ok(accounts);
    }

    private async Task<IResult> GetAccountByIdAsync(int id, IMediator mediator, CancellationToken ct)
    {
        var query = new GetAccountById { AccountId = id };
        var account = await mediator.Send(query, ct);
        return Results.Ok(account);
    }

    private async Task<IResult> CreateAccountAsync(CreateAccount AccountDto, IMediator mediator, CancellationToken ct)
    {
        var account = await mediator.Send(AccountDto, ct);
        return Results.Ok(account);
    }

    private async Task<IResult> UpdateAccountAsync(UpdateAccount accountDto, IMediator mediator, CancellationToken ct)
    {
        var account = await mediator.Send(accountDto, ct);
        return Results.Ok(account);
    }
}