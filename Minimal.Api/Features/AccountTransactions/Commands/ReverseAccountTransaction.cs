using MediatR;
using Minimal.Api.Features.AccountTransactions.Models;

namespace Minimal.Api.Features.AccountTransactions.Commands;

public class ReverseAccountTransaction : IRequest<AccountTransactionGetDto>
{
    public int Id { get; set; }
    public int AccountId { get; set; }
    public string Note { get; set; } = string.Empty;
}