using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Minimal.Api.Exceptions;
using Minimal.DataAccess;
using Minimal.Domain;

namespace Minimal.Api.Features.LoanTransactions.Commands;

public class DeleteLoanTransactionHandler : IRequestHandler<DeleteLoanTransaction>
{
    private readonly ApplicationDbContext _context;
    private readonly IStringLocalizer _localizer;

    public DeleteLoanTransactionHandler(ApplicationDbContext context, IStringLocalizer<SharedResource> localizer)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
    }

    public async Task<Unit> Handle(DeleteLoanTransaction request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var document = await _context.Documents
            .Include(d => d.DocumentItems)
            .ThenInclude(d => d.AccountDetail)
            .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);
        if (document is null)
        {
            throw new NotFoundException(_localizer.GetString("notFoundTransaction").Value);
        }

        if (document.IsActive is false)
        {
            throw new ValidationException(nameof(request.Id), _localizer.GetString("transactionIsNotActive").Value);
        }

        if (document.DocumentItems.Any(x => x.AccountDetail.IsActive == false))
        {
            throw new ValidationException(nameof(request.Id), _localizer.GetString("transactionCanNotBeDeleted").Value);
        }

        document.IsActive = false;
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}