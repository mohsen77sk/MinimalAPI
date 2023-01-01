namespace Minimal.Domain;

public class AccountType
{
    public AccountType()
    {
        Accounts = new HashSet<Account>();
    }

    public int Id { get; set; }

    public string Code { get; set; } = default!;

    public string Name { get; set; } = default!;

    public bool IsActive { get; set; }

    public virtual ICollection<Account> Accounts { get; set; }
}