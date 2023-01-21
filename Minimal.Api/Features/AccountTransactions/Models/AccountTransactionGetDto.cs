namespace Minimal.Api.Features.AccountTransactions.Models;

public class AccountTransactionGetDto
{
    public int Id { get; init; }

    public string Code { get; set; } = default!;

    public decimal Credit { get; set; }

    public decimal Debit { get; set; }

    public DateTimeOffset Date { get; set; }

    public string Note { get; set; } = default!;

    public bool Editable { get; set; }
}