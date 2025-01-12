using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Minimal.Api.Exceptions;
using Minimal.Api.Features.Loans.Models;
using Minimal.DataAccess;
using Minimal.Domain;

namespace Minimal.Api.Features.Loans.Commands;

public class UpdateLoanHandler : IRequestHandler<UpdateLoan, LoanGetDto>
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer _localizer;

    public UpdateLoanHandler(ApplicationDbContext context, IMapper mapper, IStringLocalizer<SharedResource> localizer)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
    }

    public async Task<LoanGetDto> Handle(UpdateLoan request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var loan = await _context.Loans
            .Include(a => a.Account)
            .Include(a => a.LoanType)
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);
        if (loan is null)
        {
            throw new NotFoundException(_localizer.GetString("notFoundLoan").Value);
        }

        if (loan.IsActive is false)
        {
            throw new ErrorException(_localizer.GetString("loanIsNotActive").Value);
        }

        // If change account
        if (loan.AccountId != request.AccountId)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(p => p.Id.Equals(request.AccountId), cancellationToken);
            if (account is null)
            {
                throw new ValidationException(nameof(request.AccountId), _localizer.GetString("notFoundAccount").Value);
            }
            if (account.IsActive is false)
            {
                throw new ValidationException(nameof(request.AccountId), _localizer.GetString("accountIsNotActive").Value);
            }
            loan.Account = account;
            loan.AccountId = account.Id;
        }

        loan.Note = request.Note;

        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<LoanGetDto>(loan);
    }
}