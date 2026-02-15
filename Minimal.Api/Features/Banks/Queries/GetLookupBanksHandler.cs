using MediatR;
using Microsoft.EntityFrameworkCore;
using Minimal.Api.Features.Banks.Profiles;
using Minimal.Api.Models;
using Minimal.DataAccess;

namespace Minimal.Api.Features.Banks.Queries;

public class GetLookupBankHandler : IRequestHandler<GetLookupBank, List<LookupDto>>
{
    private readonly ApplicationDbContext _context;
    private readonly BankMapper _mapper;

    public GetLookupBankHandler(ApplicationDbContext context, BankMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<List<LookupDto>> Handle(GetLookupBank request, CancellationToken cancellationToken)
    {
        var banks = await _context.Banks
            .AsNoTracking()
            .Where(b => b.IsActive == true)
            .ToListAsync(cancellationToken);

        return banks.Select(_mapper.MapToLookupDto).ToList();
    }
}