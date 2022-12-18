using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Minimal.Api.Extension;
using Minimal.Api.Features.People.Models;
using Minimal.Api.Models;
using Minimal.DataAccess;

namespace Minimal.Api.Features.People.Queries;

public class GetAllPersonHandler : IRequestHandler<GetAllPerson, PageList<PeopleGetDto>>
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAllPersonHandler(ApplicationDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<PageList<PeopleGetDto>> Handle(GetAllPerson request, CancellationToken cancellationToken)
    {
        var persons = await _context.People.AsNoTracking().ToPagedAsync(request.Page, request.PageSize, request.SortBy);
        return _mapper.Map<PageList<PeopleGetDto>>(persons);
    }
}