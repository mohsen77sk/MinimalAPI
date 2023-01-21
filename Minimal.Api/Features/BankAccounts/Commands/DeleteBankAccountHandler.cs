using MediatR;
using Microsoft.EntityFrameworkCore;
using Minimal.Api.Exceptions;
using Minimal.DataAccess;
using Minimal.Domain;

namespace Minimal.Api.Features.BankAccounts.Commands;

public class DeleteBankAccountHandler : IRequestHandler<DeleteBankAccount>
{
    private readonly ApplicationDbContext _context;

    public DeleteBankAccountHandler(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
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
            throw new NotFoundException();
        }

        _context.BankAccounts.Remove(bankAccount);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}