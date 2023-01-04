namespace Minimal.Domain;

public class FiscalYear
{
    public FiscalYear()
    {
        Documents = new List<Document>();
    }

    public int Id { get; set; }

    public string Title { get; set; } = default!;

    public DateTimeOffset BeginDate { get; set; }

    public DateTimeOffset EndDate { get; set; }

    public ICollection<Document> Documents { get; set; }
}
