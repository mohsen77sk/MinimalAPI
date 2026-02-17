using Microsoft.EntityFrameworkCore;
using Minimal.DataAccess;
using Minimal.Domain;

namespace Minimal.Api.Common.Accounting;

public static class AccountingExtensions
{
    /// <summary>
    /// Get the bank account (code "1101") from the database.
    /// </summary>
    public static async Task<AccountSubsid> GetBankAccountAsync(this ApplicationDbContext context, CancellationToken cancellationToken = default)
    {
        return await context.AccountSubsids
            .AsNoTracking()
            .FirstAsync(x => x.Code == "1101", cancellationToken);
    }

    /// <summary>
    /// Get an account detail by its code.
    /// </summary>
    public static async Task<AccountSubsid> GetAccountSubsidByCodeAsync(this ApplicationDbContext context, string code, CancellationToken cancellationToken = default)
    {
        return await context.AccountSubsids
            .AsNoTracking()
            .FirstAsync(x => x.Code == code, cancellationToken);
    }

    /// <summary>
    /// Get an account category by its code.
    /// </summary>
    public static async Task<AccountCategory> GetAccountCategoryByCodeAsync(this ApplicationDbContext context, string code, CancellationToken cancellationToken = default)
    {
        return await context.AccountCategories
            .AsNoTracking()
            .FirstAsync(ac => ac.Code == code, cancellationToken);
    }

    /// <summary>
    /// Get the current fiscal year (assuming it's the one with Id = 1) from the database.
    /// </summary>
    public static async Task<FiscalYear> GetCurrentFiscalYearAsync(this ApplicationDbContext context, CancellationToken cancellationToken = default)
    {
        return await context.FiscalYears
            .AsNoTracking()
            .FirstAsync(f => f.Id == 1, cancellationToken);
    }

    /// <summary>
    /// Get a document type by its code.
    /// </summary>
    public static async Task<DocumentType> GetDocumentTypeByCodeAsync(this ApplicationDbContext context, string code, CancellationToken cancellationToken = default)
    {
        return await context.DocumentTypes
            .AsNoTracking()
            .FirstAsync(dt => dt.Code == code, cancellationToken);
    }

    /// <summary>
    /// Calculate the balance of an account detail by summing its debit and credit amounts from the document articles.
    /// </summary>
    public static async Task<decimal> GetAccountBalanceAsync(this ApplicationDbContext context, int accountDetailId, CancellationToken cancellationToken = default)
    {
        return await context.DocumentArticles
            .AsNoTracking()
            .Where(x => x.AccountDetailId == accountDetailId && x.Document.IsActive)
            .SumAsync(x => x.Debit - x.Credit, cancellationToken);
    }

    /// <summary>
    /// Check if the document articles are balanced by comparing the total debit and credit amounts.
    /// </summary>
    public static bool IsBalanced(this IEnumerable<DocumentArticle> articles)
    {
        var debit = articles.Sum(x => x.Debit);
        var credit = articles.Sum(x => x.Credit);
        return debit == credit;
    }

    /// <summary>
    /// Calculate the total debit amount from the document articles.
    /// </summary>
    public static decimal GetTotalDebit(this IEnumerable<DocumentArticle> articles)
    {
        return articles.Sum(x => x.Debit);
    }

    /// <summary>
    /// Calculate the total credit amount from the document articles.
    /// </summary>
    public static decimal GetTotalCredit(this IEnumerable<DocumentArticle> articles)
    {
        return articles.Sum(x => x.Credit);
    }
}
