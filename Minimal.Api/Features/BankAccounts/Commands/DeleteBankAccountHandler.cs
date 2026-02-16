using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Minimal.Api.Exceptions;
using Minimal.DataAccess;
using Minimal.Domain;

namespace Minimal.Api.Features.BankAccounts.Commands;

public class DeleteBankAccountHandler : IRequestHandler<DeleteBankAccount, Unit>
{
    private readonly ApplicationDbContext _context;
    private readonly IStringLocalizer _localizer;

    public DeleteBankAccountHandler(ApplicationDbContext context, IStringLocalizer<SharedResource> localizer)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
    }

    public async Task<Unit> Handle(DeleteBankAccount request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var bankAccount = await _context.BankAccounts.FirstOrDefaultAsync(ba => ba.Id == request.Id, cancellationToken);
        if (bankAccount is null)
        {
            throw new NotFoundException(_localizer.GetString("notFoundBankAccount").Value);
        }

        _context.BankAccounts.Remove(bankAccount);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}