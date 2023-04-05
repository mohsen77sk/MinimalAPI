using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Minimal.Api.Extensions;
using Minimal.Api.Features.Loans.Models;
using Minimal.Api.Models;
using Minimal.DataAccess;

namespace Minimal.Api.Features.Loans.Queries;

public class GetAllLoanHandler : IRequestHandler<GetAllLoan, PageList<LoanGetDto>>
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAllLoanHandler(ApplicationDbContext context, IMapper mapper)
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

        var pagedLoans = await loans.ToPagedAsync(request.Page, request.PageSize, request.SortBy);

        return _mapper.Map<PageList<LoanGetDto>>(pagedLoans);
    }
}