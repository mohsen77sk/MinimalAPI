using MediatR;
using Minimal.Api.Features.People.Models;
using Minimal.Api.Features.People.Profiles;
using Minimal.DataAccess;
using Minimal.Domain;

namespace Minimal.Api.Features.People.Commands;

public class CreatePersonHandler : IRequestHandler<CreatePerson, PersonGetDto>
{
    private readonly ApplicationDbContext _context;
    private readonly PeopleMapper _mapper;

    public CreatePersonHandler(ApplicationDbContext context, PeopleMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<PersonGetDto> Handle(CreatePerson request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var personToAdd = _mapper.MapToPerson(request);
        personToAdd.IsActive = true;

        _context.People.Add(personToAdd);
        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.MapToPersonGetDto(personToAdd);
    }
}