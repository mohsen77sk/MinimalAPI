using MediatR;
using Microsoft.EntityFrameworkCore;
using Minimal.Api.Extensions;
using Minimal.Api.Features.Loans.Models;
using Minimal.Api.Features.Loans.Profiles;
using Minimal.Api.Models;
using Minimal.DataAccess;

namespace Minimal.Api.Features.Loans.Queries;

public class GetAllLoanHandler : IRequestHandler<GetAllLoan, PageList<LoanGetDto>>
{
    private readonly ApplicationDbContext _context;
    private readonly LoanMapper _mapper;

    public GetAllLoanHandler(ApplicationDbContext context, LoanMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<PageList<LoanGetDto>> Handle(GetAllLoan request, CancellationToken cancellationToken)
    {
        var loans = _context.Loans.Include(l => l.Account).Include(l => l.LoanType).AsNoTracking();

        if (request.IsActive.HasValue)
        {
            loans = loans.Where(p => p.IsActive == request.IsActive.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLower();
            loans = loans.Where(l =>
                l.Code.ToLower().Contains(search) ||
                l.LoanType.Name.ToLower().Contains(search)
            );
        }

        var pagedLoans = await loans.ToPagedAsync(request.Page, request.PageSize, request.SortBy);

        return _mapper.MapToPageList(pagedLoans);
    }
}