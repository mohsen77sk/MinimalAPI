namespace Minimal.Domain;

public class DocumentType
{
    public DocumentType()
    {
        Documents = new List<Document>();
    }

    public int Id { get; set; }

    public string Code { get; set; } = default!;

    public string Title { get; set; } = default!;

    public ICollection<Document> Documents { get; set; }
}
