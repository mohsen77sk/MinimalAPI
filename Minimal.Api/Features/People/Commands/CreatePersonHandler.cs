using AutoMapper;
using MediatR;
using Minimal.Api.Features.People.Models;
using Minimal.DataAccess;
using Minimal.Domain;

namespace Minimal.Api.Features.People.Commands;

public class CreatePersonHandler : IRequestHandler<CreatePerson, PersonGetDto>
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CreatePersonHandler(ApplicationDbContext context, IMapper mapper)
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

        var personToAdd = _mapper.Map<Person>(request);
        personToAdd.Code = GenerateCodeAsync();
        personToAdd.IsActive = true;

        _context.People.Add(personToAdd);
        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<PersonGetDto>(personToAdd);
    }

    /// <summary>
    /// Generate an new code for the person.
    /// </summary>
    private string GenerateCodeAsync()
    {
        int lastCode = Int32.Parse(_context.People.OrderByDescending(x => x.Code).FirstOrDefault()?.Code ?? "99");
        return (lastCode + 1).ToString();
    }
}