using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Minimal.Api.Features.People.Models;
using Minimal.DataAccess;
using Minimal.Domain;

namespace Minimal.Api.Features.People.Commands;

public class UpdatePersonHandler : IRequestHandler<UpdatePerson, PersonGetDto>
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public UpdatePersonHandler(ApplicationDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<PersonGetDto> Handle(UpdatePerson request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var person = await _context.People.Include(a => a.User).FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);
        if (person is null)
        {
            throw new Exception();
        }

        person.FirstName = request.FirstName;
        person.LastName = request.LastName;
        person.DateOfBirth = request.DateOfBirth;
        person.Gender = request.Gender;
        person.Note = request.Note;

        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<PersonGetDto>(person);
    }
}