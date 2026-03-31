using MediatR;
using Microsoft.EntityFrameworkCore;
using Minimal.Api.Features.Loans.Models;
using Minimal.Api.Features.Loans.Profiles;
using Minimal.DataAccess;

namespace Minimal.Api.Features.Loans.Queries;

public class GetLoanPaymentsHandler : IRequestHandler<GetLoanPayments, List<LoanPaymentsGetDto>>
{
    private readonly ApplicationDbContext _context;
    private readonly LoanMapper _mapper;

    public GetLoanPaymentsHandler(ApplicationDbContext context, LoanMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<List<LoanPaymentsGetDto>> Handle(GetLoanPayments request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var payments = await _context.LoanPayments
            .AsNoTracking()
            .Include(lp => lp.Document)
            .Where(lp => lp.LoanId == request.LoanId)
            .OrderBy(lp => lp.PaymentDate)
            .ToListAsync(cancellationToken);

        return payments.Select(_mapper.MapToLoanPaymentGetDto).ToList();
    }
}