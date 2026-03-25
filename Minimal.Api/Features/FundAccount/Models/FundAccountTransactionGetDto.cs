namespace Minimal.Api.Features.FundAccount.Models;

public class FundAccountTransactionGetDto
{
    public int Id { get; init; }

    public string Code { get; set; } = default!;

    public DateTimeOffset Date { get; set; }

    public decimal Credit { get; set; }

    public decimal Debit { get; set; }

    public string Note { get; set; } = default!;
}