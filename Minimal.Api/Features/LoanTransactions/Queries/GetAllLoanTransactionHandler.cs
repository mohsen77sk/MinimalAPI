using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Minimal.Api.Exceptions;
using Minimal.Api.Extensions;
using Minimal.Api.Features.LoanTransactions.Models;
using Minimal.Api.Models;
using Minimal.DataAccess;

namespace Minimal.Api.Features.LoanTransactions.Queries;

public class GetAllLoanTransactionHandler : IRequestHandler<GetAllLoanTransaction, PageList<LoanTransactionGetDto>>
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAllLoanTransactionHandler(ApplicationDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<PageList<LoanTransactionGetDto>> Handle(GetAllLoanTransaction request, CancellationToken cancellationToken)
    {
        var Loan = await _context.Loans
            .AsNoTracking()
            .Include(x => x.AccountDetail)
            .Select(a => new
            {
                Id = a.Id,
                AccountDetailId = a.AccountDetail.Id
            })
            .FirstOrDefaultAsync(a => a.Id == request.LoanId, cancellationToken);
        if (Loan is null)
        {
            throw new NotFoundException();
        }

        var documents = await _context.Documents
            .AsNoTracking()
            .Include(d => d.DocumentType)
            .Include(d => d.DocumentItems)
            .ThenInclude(di => di.AccountDetail)
            .Where(d => d.IsActive == true && d.DocumentItems.Any(x => x.AccountDetailId == Loan.AccountDetailId))
            .ToPagedAsync(request.Page, request.PageSize, request.SortBy);

        return _mapper.Map<PageList<LoanTransactionGetDto>>(documents);
    }
}