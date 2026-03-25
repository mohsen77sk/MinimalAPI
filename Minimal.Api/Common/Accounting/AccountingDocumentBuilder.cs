using Minimal.Domain;

namespace Minimal.Api.Common.Accounting;

public class AccountingDocumentBuilder
{
    private readonly Document _document;

    public AccountingDocumentBuilder(FiscalYear fiscalYear, DocumentType type, DateTimeOffset date, string? note = null)
    {
        _document = new Document
        {
            FiscalYear = fiscalYear,
            DocumentType = type,
            Date = date,
            DocumentItems = new List<DocumentArticle>(),
            Note = note ?? string.Empty,
        };
    }

    public AccountingDocumentBuilder Debit(decimal amount, AccountSubsid account, AccountDetail? detail = null, string? note = null)
    {
        _document.DocumentItems.Add(new DocumentArticle
        {
            AccountSubsid = account,
            AccountDetail = detail,
            Debit = amount,
            Credit = 0,
            Note = note ?? string.Empty
        });

        return this;
    }

    public AccountingDocumentBuilder Credit(decimal amount, AccountSubsid account, AccountDetail? detail = null, string? note = null)
    {
        _document.DocumentItems.Add(new DocumentArticle
        {
            AccountSubsid = account,
            AccountDetail = detail,
            Debit = 0,
            Credit = amount,
            Note = note ?? string.Empty
        });

        return this;
    }

    public Document Build()
    {
        return _document;
    }
}