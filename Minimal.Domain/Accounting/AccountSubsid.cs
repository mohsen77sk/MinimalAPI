namespace Minimal.Domain;

public class AccountSubsid
{
    public AccountSubsid()
    {
        DocumentArticleList = [];
        AccountCategoryList = [];
    }

    public int Id { get; set; }

    public string Code { get; set; } = default!;

    public string Title { get; set; } = default!;

    public int AccountEssenceId { get; set; }
    public AccountEssence AccountEssence { get; set; } = default!;

    public int AccountLedgerId { get; set; }
    public AccountLedger AccountLedger { get; set; } = default!;

    public bool IsSystemic { get; set; }

    public ICollection<DocumentArticle> DocumentArticleList { get; set; }

    public ICollection<AccountCategory> AccountCategoryList { get; set; }
}