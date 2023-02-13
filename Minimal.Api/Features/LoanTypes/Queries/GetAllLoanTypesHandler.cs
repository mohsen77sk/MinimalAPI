using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Minimal.Api.Extensions;
using Minimal.Api.Features.LoanTypes.Models;
using Minimal.Api.Models;
using Minimal.DataAccess;

namespace Minimal.Api.Features.LoanTypes.Queries;

public class GetAllLoanTypeHandler : IRequestHandler<GetAllLoanType, PageList<LoanTypeGetDto>>
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAllLoanTypeHandler(ApplicationDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<PageList<LoanTypeGetDto>> Handle(GetAllLoanType request, CancellationToken cancellationToken)
    {
        var loanTypes = _context.LoanTypes.AsNoTracking();

        if (request.IsActive is not null)
        {
            loanTypes = loanTypes.Where(p => p.IsActive == request.IsActive);
        }

        return _mapper.Map<PageList<LoanTypeGetDto>>(
            await loanTypes.ToPagedAsync(request.Page, request.PageSize, request.SortBy)
        );
    }
}