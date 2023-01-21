using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Minimal.Api.Exceptions;
using Minimal.Api.Features.People.Models;
using Minimal.DataAccess;

namespace Minimal.Api.Features.People.Queries;

public class GetPersonByIdHandler : IRequestHandler<GetPersonById, PersonGetDto>
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetPersonByIdHandler(ApplicationDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
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
            throw new NotFoundException();
        }

        return _mapper.Map<PersonGetDto>(person);
    }
}