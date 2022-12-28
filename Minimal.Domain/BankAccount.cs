namespace Minimal.Domain;

public class BankAccount
{
    public int Id { get; set; }

    public int BankId { get; set; }
    public Bank Bank { get; set; } = default!;

    public string BranchCode { get; set; } = default!;

    public string BranchName { get; set; } = default!;

    public string AccountNumber { get; set; } = default!;

    public string CardNumber { get; set; } = default!;

    public string Iban { get; set; } = default!;

    public bool IsActive { get; set; }

    public int PersonId { get; set; }
    public Person Person { get; set; } = default!;
}
