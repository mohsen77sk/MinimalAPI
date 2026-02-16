using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Minimal.Api.Exceptions;
using Minimal.Api.Features.People.Models;
using Minimal.Api.Features.People.Profiles;
using Minimal.DataAccess;

namespace Minimal.Api.Features.People.Queries;

public class GetPersonByIdHandler : IRequestHandler<GetPersonById, PersonGetDto>
{
    private readonly ApplicationDbContext _context;
    private readonly PeopleMapper _mapper;
    private readonly IStringLocalizer _localizer;

    public GetPersonByIdHandler(ApplicationDbContext context, PeopleMapper mapper, IStringLocalizer<SharedResource> localizer)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
    }

    public async Task<PersonGetDto> Handle(GetPersonById request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var person = await _context.People.AsNoTracking().FirstOrDefaultAsync(p => p.Id == request.PersonId, cancellationToken);
        if (person is null)
        {
            throw new NotFoundException(_localizer.GetString("notFoundPerson").Value);
        }

        return _mapper.MapToPersonGetDto(person);
    }
}