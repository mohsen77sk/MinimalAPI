using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Minimal.Api.Exceptions;
using Minimal.Api.Features.People.Models;
using Minimal.Api.Features.People.Profiles;
using Minimal.DataAccess;
using Minimal.Domain;

namespace Minimal.Api.Features.People.Commands;

public class UpdateStatusPersonHandler : IRequestHandler<UpdateStatusPerson, PersonGetDto>
{
    private readonly ApplicationDbContext _context;
    private readonly PeopleMapper _mapper;
    private readonly IStringLocalizer _localizer;

    public UpdateStatusPersonHandler(ApplicationDbContext context, PeopleMapper mapper, IStringLocalizer<SharedResource> localizer)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
    }

    public async Task<PersonGetDto> Handle(UpdateStatusPerson request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var person = await _context.People
            .Include(p => p.User)
            .Include(p => p.Accounts)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
        if (person is null)
        {
            throw new NotFoundException(_localizer.GetString("notFoundPerson").Value);
        }

        if (request.IsActive is false && person.Accounts.Any(a => a.IsActive == true))
        {
            throw new ErrorException(_localizer.GetString("personHasActiveAccount").Value);
        }

        person.IsActive = request.IsActive;

        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.MapToPersonGetDto(person);
    }
}