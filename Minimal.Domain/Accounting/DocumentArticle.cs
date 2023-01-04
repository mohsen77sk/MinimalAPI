namespace Minimal.Domain;

public class DocumentArticle
{
    public int Id { get; set; }

    public int DocumentId { get; set; }
    public Document Document { get; set; } = default!;

    public int AccountSubsidId { get; set; }
    public AccountSubsid AccountSubsid { get; set; } = default!;

    public int AccountDetailId { get; set; }
    public AccountDetail AccountDetail { get; set; } = default!;

    public decimal Credit { get; set; }

    public decimal Debit { get; set; }

    public string Note { get; set; } = default!;
}