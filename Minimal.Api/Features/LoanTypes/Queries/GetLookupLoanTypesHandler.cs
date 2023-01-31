using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Minimal.Api.Models;
using Minimal.DataAccess;

namespace Minimal.Api.Features.LoanTypes.Queries;

public class GetLookupLoanTypeHandler : IRequestHandler<GetLookupLoanType, List<LookupDto>>
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetLookupLoanTypeHandler(ApplicationDbContext context, IMapper mapper)
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

        return _mapper.Map<List<LookupDto>>(loanTypes);
    }
}