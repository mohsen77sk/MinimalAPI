using MediatR;
using Minimal.Api.Contracts;
using Minimal.Api.Features.People.Commands;
using Minimal.Api.Features.People.Models;
using Minimal.Api.Features.People.Queries;
using Minimal.Api.Models;

namespace Minimal.Api.Features.People;

public class PeopleModule : IModule
{
    public IEndpointRouteBuilder RegisterEndpoints(IEndpointRouteBuilder endpoints)
    {
        var persons = endpoints.MapGroup("/api/person").WithDisplayName("Persons").RequireAuthorization();

        persons.MapGet("/all", GetPersonsAsync)
            .Produces<PageList<PersonGetDto>>()
            .Produces(500);

        persons.MapGet("/{id}", GetPersonByIdAsync)
            .Produces<PersonGetDto>()
            .Produces(404)
            .Produces(500);

        persons.MapPost("/", CreatePersonAsync)
            .Produces<PersonGetDto>()
            .Produces<ValidationError>(400)
            .Produces(500);

        persons.MapPut("/", UpdatePersonAsync)
            .Produces<PersonGetDto>()
            .Produces<ValidationError>(400)
            .Produces(404)
            .Produces(500);

        persons.MapPatch("/", UpdatePersonStatusAsync)
            .Produces<PersonGetDto>()
            .Produces<ValidationError>(400)
            .Produces(404)
            .Produces(500);

        return endpoints;
    }

    private async Task<IResult> GetPersonsAsync([AsParameters] PagingData request, IMediator mediator, CancellationToken ct)
    {
        var query = new GetAllPerson
        {
            Page = request.Page,
            PageSize = request.PageSize,
            SortBy = request.SortBy
        };
        var persons = await mediator.Send(query, ct);
        return Results.Ok(persons);
    }

    private async Task<IResult> GetPersonByIdAsync(int id, IMediator mediator, CancellationToken ct)
    {
        var query = new GetPersonById { PersonId = id };
        var person = await mediator.Send(query, ct);
        return Results.Ok(person);
    }

    private async Task<IResult> CreatePersonAsync(CreatePerson personDto, IMediator mediator, CancellationToken ct)
    {
        var person = await mediator.Send(personDto, ct);
        return Results.Ok(person);
    }

    private async Task<IResult> UpdatePersonAsync(UpdatePerson personDto, IMediator mediator, CancellationToken ct)
    {
        var person = await mediator.Send(personDto, ct);
        return Results.Ok(person);
    }

    private async Task<IResult> UpdatePersonStatusAsync(UpdateStatusPerson personDto, IMediator mediator, CancellationToken ct)
    {
        var person = await mediator.Send(personDto, ct);
        return Results.Ok(person);
    }

}