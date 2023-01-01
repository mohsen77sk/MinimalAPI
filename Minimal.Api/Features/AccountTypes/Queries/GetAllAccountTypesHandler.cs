using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Minimal.Api.Features.AccountTypes.Models;
using Minimal.DataAccess;

namespace Minimal.Api.Features.AccountTypes.Queries;

public class GetAllAccountTypeHandler : IRequestHandler<GetAllAccountType, List<AccountTypeGetDto>>
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAllAccountTypeHandler(ApplicationDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<List<AccountTypeGetDto>> Handle(GetAllAccountType request, CancellationToken cancellationToken)
    {
        var accountTypes = await _context.AccountTypes.AsNoTracking().ToListAsync(cancellationToken);
        return _mapper.Map<List<AccountTypeGetDto>>(accountTypes);
    }
}