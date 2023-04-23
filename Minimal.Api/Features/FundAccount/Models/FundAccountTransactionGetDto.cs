namespace Minimal.Api.Features.FundAccount.Models;

public class FundAccountTransactionGetDto
{
    public int Id { get; init; }

    public DateTimeOffset Date { get; set; }

    public decimal Credit { get; set; }

    public decimal debit { get; set; }

    public string Note { get; set; } = default!;
}