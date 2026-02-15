using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Minimal.Api.Exceptions;
using Minimal.Api.Features.Loans.Models;
using Minimal.Api.Features.Loans.Profiles;
using Minimal.DataAccess;

namespace Minimal.Api.Features.Loans.Queries;

public class GetLoanByIdHandler : IRequestHandler<GetLoanById, LoanGetDto>
{
    private readonly ApplicationDbContext _context;
    private readonly LoanMapper _mapper;
    private readonly IStringLocalizer _localizer;

    public GetLoanByIdHandler(ApplicationDbContext context, LoanMapper mapper, IStringLocalizer<SharedResource> localizer)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
    }

    public async Task<LoanGetDto> Handle(GetLoanById request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var loan = await _context.Loans
            .AsNoTracking()
            .Include(l => l.Account)
            .Include(l => l.LoanType)
            .FirstOrDefaultAsync(a => a.Id == request.LoanId, cancellationToken);

        if (loan is null)
        {
            throw new NotFoundException(_localizer.GetString("notFoundLoan").Value);
        }

        return _mapper.MapToLoanGetDto(loan);
    }
}