using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Minimal.Api.Models;
using Minimal.DataAccess;

namespace Minimal.Api.Features.Accounts.Queries;

public class GetLookupAccountsHandler : IRequestHandler<GetLookupAccounts, List<LookupDto>>
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetLookupAccountsHandler(ApplicationDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<List<LookupDto>> Handle(GetLookupAccounts request, CancellationToken cancellationToken)
    {
        var accounts = await _context.Accounts
            .Include(a => a.AccountType)
            .Include(a => a.People)
            .AsNoTracking()
            .Where(at => at.IsActive == true)
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<LookupDto>>(accounts);
    }
}