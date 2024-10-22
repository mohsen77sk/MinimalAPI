using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Minimal.Api.Models;
using Minimal.DataAccess;

namespace Minimal.Api.Features.People.Queries;

public class GetLookupPersonsHandler : IRequestHandler<GetLookupPersons, List<LookupDto>>
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetLookupPersonsHandler(ApplicationDbContext context, IMapper mapper)
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

        return _mapper.Map<List<LookupDto>>(persons);
    }
}