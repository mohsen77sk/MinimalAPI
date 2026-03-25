using Minimal.Domain;

namespace Minimal.Api.Common.Accounting;

public class AccountingDocumentReversalBuilder
{
    private readonly Document _document;

    public AccountingDocumentReversalBuilder(Document sourceDocument, DocumentType type, string? note = null)
    {
        _document = new Document
        {
            FiscalYearId = sourceDocument.FiscalYearId,
            DocumentType = type,
            Date = sourceDocument.Date.AddMinutes(1),
            DocumentItems = [],
            Note = note ?? string.Empty,
            RefDocument = sourceDocument
        };

        foreach (var detail in sourceDocument.DocumentItems)
        {
            _document.DocumentItems.Add(new DocumentArticle
            {
                AccountSubsidId = detail.AccountSubsidId,
                AccountDetailId = detail.AccountDetailId,
                Credit = detail.Debit,
                Debit = detail.Credit
            });
        }
    }

    public Document Build()
    {
        return _document;
    }
}