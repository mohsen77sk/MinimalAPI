using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Minimal.Api.Exceptions;
using Minimal.Api.Features.People.Models;
using Minimal.DataAccess;
using Minimal.Domain;

namespace Minimal.Api.Features.People.Commands;

public class UpdatePersonHandler : IRequestHandler<UpdatePerson, PersonGetDto>
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer _localizer;

    public UpdatePersonHandler(ApplicationDbContext context, IMapper mapper, IStringLocalizer<SharedResource> localizer)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
    }

    public async Task<PersonGetDto> Handle(UpdatePerson request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var person = await _context.People.Include(p => p.User).FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
        if (person is null)
        {
            throw new NotFoundException(_localizer.GetString("notFoundPerson").Value);
        }

        person.FirstName = request.FirstName;
        person.LastName = request.LastName;
        person.NationalCode = request.NationalCode;
        person.Birthday = request.Birthday;
        person.Gender = (byte)request.Gender;
        person.Note = request.Note;

        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<PersonGetDto>(person);
    }
}