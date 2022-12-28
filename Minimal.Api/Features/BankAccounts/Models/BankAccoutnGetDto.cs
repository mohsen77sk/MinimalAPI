namespace Minimal.Api.Features.BankAccounts.Models;

public class BankAccountGetDto
{
    public int Id { get; init; }

    public int PersonId { get; set; }

    public string PersonName { get; set; } = default!;

    public int BankId { get; set; }

    public string BankName { get; set; } = default!;

    public string BranchCode { get; set; } = default!;

    public string BranchName { get; set; } = default!;

    public string AccountNumber { get; set; } = default!;

    public string CardNumber { get; set; } = default!;

    public string Iban { get; set; } = default!;

    public bool IsActive { get; set; }
}
