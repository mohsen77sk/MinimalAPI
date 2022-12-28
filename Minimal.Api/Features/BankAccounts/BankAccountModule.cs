
using MediatR;
using Minimal.Api.Contracts;
using Minimal.Api.Features.BankAccounts.Commands;
using Minimal.Api.Features.BankAccounts.Models;
using Minimal.Api.Features.BankAccounts.Queries;
using Minimal.Api.Models;

namespace Minimal.Api.Features.BankAccounts;

public class BankAccountModule : IModule
{
    public IEndpointRouteBuilder RegisterEndpoints(IEndpointRouteBuilder endpoints)
    {
        var bankAccounts = endpoints.MapGroup("/api/bankAccount").WithDisplayName("BankAccounts");

        bankAccounts.MapGet("/all", GetBankAccountsAsync)
            .Produces<PageList<BankAccountGetDto>>()
            .Produces(500);

        bankAccounts.MapGet("/{id}", GetBankAccountByIdAsync)
            .Produces<BankAccountGetDto>()
            .Produces(404)
            .Produces(500);

        bankAccounts.MapPost("/", CreateBankAccountAsync)
            .Produces<BankAccountGetDto>()
            .Produces<ValidationError>(400)
            .Produces(500);

        bankAccounts.MapPut("/", UpdateBankAccountAsync)
            .Produces<BankAccountGetDto>()
            .Produces<ValidationError>(400)
            .Produces(404)
            .Produces(500);

        return endpoints;
    }

    private async Task<IResult> GetBankAccountsAsync([AsParameters] PagingData request, IMediator mediator, CancellationToken ct)
    {
        var query = new GetAllBankAccount
        {
            Page = request.Page,
            PageSize = request.PageSize,
            SortBy = request.SortBy
        };
        var persons = await mediator.Send(query, ct);
        return Results.Ok(persons);
    }

    private async Task<IResult> GetBankAccountByIdAsync(int id, IMediator mediator, CancellationToken ct)
    {
        var query = new GetBankAccountById { BankAccountId = id };
        var person = await mediator.Send(query, ct);
        return Results.Ok(person);
    }

    private async Task<IResult> CreateBankAccountAsync(CreateBankAccount bankAccountDto, IMediator mediator, CancellationToken ct)
    {
        var person = await mediator.Send(bankAccountDto, ct);
        return Results.Ok(person);
    }

    private async Task<IResult> UpdateBankAccountAsync(UpdateBankAccount bankAccountDto, IMediator mediator, CancellationToken ct)
    {
        var person = await mediator.Send(bankAccountDto, ct);
        return Results.Ok(person);
    }
}
