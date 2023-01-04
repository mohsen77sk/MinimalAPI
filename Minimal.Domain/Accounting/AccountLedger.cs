namespace Minimal.Domain;

public class AccountLedger
{
    public AccountLedger()
    {
        AccountSubsids = new List<AccountSubsid>();
    }

    public int Id { get; set; }

    public string Code { get; set; } = default!;

    public string Title { get; set; } = default!;

    public int AccountGroupId { get; set; }
    public AccountGroup AccountGroup { get; set; } = default!;

    public bool IsSystemic { get; set; }

    public ICollection<AccountSubsid> AccountSubsids { get; set; }
}