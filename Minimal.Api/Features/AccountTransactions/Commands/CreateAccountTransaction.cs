using MediatR;
using Minimal.Api.Features.AccountTransactions.Models;

namespace Minimal.Api.Features.AccountTransactions.Commands;

public class CreateAccountTransaction : IRequest<AccountTransactionGetDto>
{
    public int AccountId { get; set; }
    public decimal Amount { get; set; }
    public TransactionTypeEnum TransactionType { get; set; }
    public DateTimeOffset Date { get; set; }
    public string Note { get; set; } = string.Empty;
}

public enum TransactionTypeEnum
{
    Credit = 1,
    Debit = 2
}