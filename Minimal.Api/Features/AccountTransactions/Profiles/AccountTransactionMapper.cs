using Minimal.Api.Features.AccountTransactions.Models;
using Minimal.Api.Models;
using Minimal.Domain;

namespace Minimal.Api.Features.AccountTransactions.Profiles;

public class AccountTransactionMapper
{
    public AccountTransactionGetDto MapToAccountTransactionGetDto(Document source) =>
        new AccountTransactionGetDto
        {
            Id = source.Id,
            Code = source.Code,
            Date = source.Date,
            Note = source.Note ?? string.Empty,
            Credit = GetCredit(source),
            Debit = GetDebit(source),
            Editable = GetEditable(source)
        };

    public PageList<AccountTransactionGetDto> MapToPageList(PageList<Document> source) =>
        new PageList<AccountTransactionGetDto>(
            source.Items.Select(MapToAccountTransactionGetDto).ToList(),
            source.Total,
            source.Page,
            source.PageSize);

    private static decimal GetCredit(Document src) =>
        src.DocumentType != null && new[] { "10", "12" }.Contains(src.DocumentType.Code)
            ? (src.DocumentItems?.Sum(x => x.Credit) ?? 0)
            : 0;

    private static decimal GetDebit(Document src) =>
        src.DocumentType != null && new[] { "11", "13" }.Contains(src.DocumentType.Code)
            ? (src.DocumentItems?.Sum(x => x.Debit) ?? 0)
            : 0;

    private static bool GetEditable(Document src) =>
        src.DocumentItems?.Any(x => x.AccountDetail?.IsActive == false) != true;
}
