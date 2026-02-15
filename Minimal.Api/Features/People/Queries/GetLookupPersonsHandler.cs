using MediatR;
using Microsoft.EntityFrameworkCore;
using Minimal.Api.Features.People.Profiles;
using Minimal.Api.Models;
using Minimal.DataAccess;

namespace Minimal.Api.Features.People.Queries;

public class GetLookupPersonsHandler : IRequestHandler<GetLookupPersons, List<LookupDto>>
{
    private readonly ApplicationDbContext _context;
    private readonly PeopleMapper _mapper;

    public GetLookupPersonsHandler(ApplicationDbContext context, PeopleMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<List<LookupDto>> Handle(GetLookupPersons request, CancellationToken cancellationToken)
    {
        var persons = await _context.People
            .AsNoTracking()
            .Where(at => at.IsActive == true)
            .ToListAsync(cancellationToken);

        return persons.Select(_mapper.MapToLookupDto).ToList();
    }
}