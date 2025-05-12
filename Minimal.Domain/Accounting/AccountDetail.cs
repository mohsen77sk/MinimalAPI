namespace Minimal.Domain;

public class AccountDetail
{
    public AccountDetail()
    {
        DocumentArticleList = [];
    }

    public int Id { get; set; }

    public string Code { get; set; } = default!;

    public string Title { get; set; } = default!;

    public int Level { get; set; }

    public int AccountCategoryId { get; set; }
    public AccountCategory AccountCategory { get; set; } = default!;

    public bool IsActive { get; set; }

    public int? AccountId { get; set; }
    public Account? Account { get; set; }

    public int? LoanId { get; set; }
    public Loan? Loan { get; set; }

    public ICollection<DocumentArticle> DocumentArticleList { get; set; }
}