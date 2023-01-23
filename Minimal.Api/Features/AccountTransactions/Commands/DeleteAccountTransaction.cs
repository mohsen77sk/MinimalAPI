using MediatR;

namespace Minimal.Api.Features.AccountTransactions.Commands;

public class DeleteAccountTransaction : IRequest<Unit>
{
    public int Id { get; set; }
}