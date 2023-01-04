namespace Minimal.Domain;

public class AccountCategory
{
    public AccountCategory()
    {
        AccountSubsidList = new List<AccountSubsid>();
        AccountDetailList = new List<AccountDetail>();
    }

    public int Id { get; set; }

    public string Code { get; set; } = default!;

    public string Title { get; set; } = default!;

    public ICollection<AccountSubsid> AccountSubsidList { get; set; }

    public ICollection<AccountDetail> AccountDetailList { get; set; }
}