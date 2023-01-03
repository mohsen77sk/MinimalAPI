namespace Minimal.Domain;

public class AccountType
{
    public AccountType()
    {
        Accounts = new List<Account>();
    }

    public int Id { get; set; }

    public string Code { get; set; } = default!;

    public string Name { get; set; } = default!;

    public bool IsActive { get; set; }

    public ICollection<Account> Accounts { get; set; }
}