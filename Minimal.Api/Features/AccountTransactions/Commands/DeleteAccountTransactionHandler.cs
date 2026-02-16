using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Minimal.Api.Exceptions;
using Minimal.DataAccess;
using Minimal.Domain;

namespace Minimal.Api.Features.AccountTransactions.Commands;

public class DeleteAccountTransactionHandler : IRequestHandler<DeleteAccountTransaction, Unit>
{
    private readonly ApplicationDbContext _context;
    private readonly IStringLocalizer _localizer;

    public DeleteAccountTransactionHandler(ApplicationDbContext context, IStringLocalizer<SharedResource> localizer)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
    }

    public async Task<Unit> Handle(DeleteAccountTransaction request, CancellationToken cancellationToken)
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
            throw new ErrorException(_localizer.GetString("transactionIsNotActive").Value);
        }

        if (document.DocumentItems.Any(x => x.AccountDetail?.IsActive == false))
        {
            throw new ErrorException(_localizer.GetString("transactionCanNotBeDeleted").Value);
        }

        document.IsActive = false;
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}