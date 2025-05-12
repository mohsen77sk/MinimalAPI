namespace Minimal.Domain;

public class Bank
{
    public Bank()
    {
        BankAccounts = [];
    }

    public int Id { get; set; }

    public string Code { get; set; } = default!;

    public string Name { get; set; } = default!;

    public bool IsActive { get; set; }

    public ICollection<BankAccount> BankAccounts { get; set; }

}