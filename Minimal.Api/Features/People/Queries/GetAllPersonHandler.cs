using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Minimal.Api.Extensions;
using Minimal.Api.Features.People.Models;
using Minimal.Api.Models;
using Minimal.DataAccess;

namespace Minimal.Api.Features.People.Queries;

public class GetAllPersonHandler : IRequestHandler<GetAllPerson, PageList<PersonGetDto>>
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAllPersonHandler(ApplicationDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<PageList<PersonGetDto>> Handle(GetAllPerson request, CancellationToken cancellationToken)
    {
        var persons = _context.People.AsNoTracking();

        if (request.IsActive.HasValue)
        {
            persons = persons.Where(p => p.IsActive == request.IsActive.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLower();
            persons = persons.Where(p =>
                p.Code.ToLower().Contains(search) ||
                p.FirstName.ToLower().Contains(search) ||
                p.LastName.ToLower().Contains(search)
            );
        }

        var pagedPersons = await persons.ToPagedAsync(request.Page, request.PageSize, request.SortBy);

        return _mapper.Map<PageList<PersonGetDto>>(pagedPersons);
    }
}