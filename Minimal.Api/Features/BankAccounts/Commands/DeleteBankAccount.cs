using MediatR;

namespace Minimal.Api.Features.BankAccounts.Commands;

public class DeleteBankAccount : IRequest<Unit>
{
    public int Id { get; set; }
}