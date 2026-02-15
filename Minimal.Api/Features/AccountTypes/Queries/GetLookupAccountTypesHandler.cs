using MediatR;
using Microsoft.EntityFrameworkCore;
using Minimal.Api.Features.AccountTypes.Profiles;
using Minimal.Api.Models;
using Minimal.DataAccess;

namespace Minimal.Api.Features.AccountTypes.Queries;

public class GetLookupAccountTypeHandler : IRequestHandler<GetLookupAccountType, List<LookupDto>>
{
    private readonly ApplicationDbContext _context;
    private readonly AccountTypeMapper _mapper;

    public GetLookupAccountTypeHandler(ApplicationDbContext context, AccountTypeMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<List<LookupDto>> Handle(GetLookupAccountType request, CancellationToken cancellationToken)
    {
        var accountTypes = await _context.AccountTypes
            .AsNoTracking()
            .Where(at => at.IsActive == true)
            .ToListAsync(cancellationToken);

        return accountTypes.Select(_mapper.MapToLookupDto).ToList();
    }
}