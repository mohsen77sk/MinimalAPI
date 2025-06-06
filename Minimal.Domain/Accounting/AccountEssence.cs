namespace Minimal.Domain;

public class AccountEssence
{
    public AccountEssence()
    {
        AccountSubsids = [];
    }

    public int Id { get; set; }

    public string Code { get; set; } = default!;

    public string Title { get; set; } = default!;

    public ICollection<AccountSubsid> AccountSubsids { get; set; }
}