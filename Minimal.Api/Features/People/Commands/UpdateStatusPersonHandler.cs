using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Minimal.Api.Exceptions;
using Minimal.Api.Features.People.Models;
using Minimal.DataAccess;
using Minimal.Domain;

namespace Minimal.Api.Features.People.Commands;

public class UpdateStatusPersonHandler : IRequestHandler<UpdateStatusPerson, PersonGetDto>
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer _localizer;

    public UpdateStatusPersonHandler(ApplicationDbContext context, IMapper mapper, IStringLocalizer<SharedResource> localizer)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _localizer = localizer ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<PersonGetDto> Handle(UpdateStatusPerson request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var person = await _context.People
            .Include(a => a.User)
            .Include(a => a.Accounts)
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);
        if (person is null)
        {
            throw new NotFoundException();
        }

        if (request.IsActive is false && person.Accounts.Any(x => x.IsActive == true))
        {
            throw new ValidationException(nameof(request.IsActive), _localizer.GetString("personHasActiveAccount").Value);
        }

        person.IsActive = request.IsActive;

        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<PersonGetDto>(person);
    }
}