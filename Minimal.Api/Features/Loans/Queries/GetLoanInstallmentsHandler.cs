using MediatR;
using Microsoft.EntityFrameworkCore;
using Minimal.Api.Features.Loans.Models;
using Minimal.Api.Features.Loans.Profiles;
using Minimal.DataAccess;

namespace Minimal.Api.Features.Loans.Queries;

public class GetLoanInstallmentsHandler : IRequestHandler<GetLoanInstallments, List<LoanInstallmentsGetDto>>
{
    private readonly ApplicationDbContext _context;
    private readonly LoanMapper _mapper;

    public GetLoanInstallmentsHandler(ApplicationDbContext context, LoanMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<List<LoanInstallmentsGetDto>> Handle(GetLoanInstallments request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var installments = await _context.LoanInstallments
            .AsNoTracking()
            .Where(li => li.LoanId == request.LoanId)
            .OrderBy(li => li.Number)
            .ToListAsync(cancellationToken);

        return installments.Select(_mapper.MapToLoanInstalmentGetDto).ToList();
    }
}