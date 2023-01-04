namespace Minimal.Domain;

public class AccountGroup
{
    public AccountGroup()
    {
        AccountLedgers = new List<AccountLedger>();
    }

    public int Id { get; set; }

    public string Code { get; set; } = default!;

    public string Title { get; set; } = default!;

    public bool IsSystemic { get; set; }

    public ICollection<AccountLedger> AccountLedgers { get; set; }
}