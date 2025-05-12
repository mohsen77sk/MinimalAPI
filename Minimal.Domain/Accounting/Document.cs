namespace Minimal.Domain;

public class Document
{
    public Document()
    {
        DocumentItems = [];
    }

    public int Id { get; set; }

    public string Code { get; set; } = default!;

    public int FiscalYearId { get; set; }
    public FiscalYear FiscalYear { get; set; } = default!;

    public int DocumentTypeId { get; set; }
    public DocumentType DocumentType { get; set; } = default!;

    public DateTimeOffset Date { get; set; }

    public string Note { get; set; } = default!;

    public bool IsConfirmed { get; set; }

    public bool IsActive { get; set; }

    public ICollection<DocumentArticle> DocumentItems { get; set; }
}