namespace Minimal.Api.Features.LoanTransactions.Models;

public class LoanTransactionGetDto
{
    public int Id { get; init; }

    public string Code { get; set; } = default!;

    public decimal Amount { get; set; }

    public DateTimeOffset Date { get; set; }

    public string Note { get; set; } = default!;

    public bool Editable { get; set; }
}