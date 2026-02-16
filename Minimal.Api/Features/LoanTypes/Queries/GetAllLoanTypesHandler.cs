using MediatR;
using Microsoft.EntityFrameworkCore;
using Minimal.Api.Extensions;
using Minimal.Api.Features.LoanTypes.Models;
using Minimal.Api.Features.LoanTypes.Profiles;
using Minimal.Api.Models;
using Minimal.DataAccess;

namespace Minimal.Api.Features.LoanTypes.Queries;

public class GetAllLoanTypeHandler : IRequestHandler<GetAllLoanType, PageList<LoanTypeGetDto>>
{
    private readonly ApplicationDbContext _context;
    private readonly LoanTypeMapper _mapper;

    public GetAllLoanTypeHandler(ApplicationDbContext context, LoanTypeMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<PageList<LoanTypeGetDto>> Handle(GetAllLoanType request, CancellationToken cancellationToken)
    {
        var loanTypes = _context.LoanTypes.AsNoTracking();

        if (request.IsActive.HasValue)
        {
            loanTypes = loanTypes.Where(p => p.IsActive == request.IsActive.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLower();
            loanTypes = loanTypes.Where(p =>
                p.Code.ToLower().Contains(search) ||
                p.Name.ToLower().Contains(search)
            );
        }

        var pagedLoanTypes = await loanTypes.ToPagedAsync(request.Page, request.PageSize, request.SortBy);

        return _mapper.MapToPageList(pagedLoanTypes);
    }
}