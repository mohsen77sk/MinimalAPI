using MediatR;
using Microsoft.EntityFrameworkCore;
using Minimal.Api.Features.LoanTypes.Profiles;
using Minimal.Api.Models;
using Minimal.DataAccess;

namespace Minimal.Api.Features.LoanTypes.Queries;

public class GetLookupLoanTypeHandler : IRequestHandler<GetLookupLoanType, List<LookupDto>>
{
    private readonly ApplicationDbContext _context;
    private readonly LoanTypeMapper _mapper;

    public GetLookupLoanTypeHandler(ApplicationDbContext context, LoanTypeMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<List<LookupDto>> Handle(GetLookupLoanType request, CancellationToken cancellationToken)
    {
        var loanTypes = await _context.LoanTypes
            .AsNoTracking()
            .Where(at => at.IsActive == true)
            .ToListAsync(cancellationToken);

        return loanTypes.Select(_mapper.MapToLookupDto).ToList();
    }
}