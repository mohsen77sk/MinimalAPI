using Minimal.Api.Features.AccountTransactions.Models;
using Minimal.Api.Models;
using Minimal.Domain;

namespace Minimal.Api.Features.AccountTransactions.Profiles;

public class AccountTransactionMapper
{
    public AccountTransactionGetDto MapToAccountTransactionGetDto(DocumentArticle source) =>
        new AccountTransactionGetDto
        {
            Id = source.Id,
            Code = source.Document.Code,
            Date = source.Document.Date,
            Note = source.Document.Note,
            Credit = source.Credit,
            Debit = source.Debit,
        };

    public PageList<AccountTransactionGetDto> MapToPageList(PageList<DocumentArticle> source) =>
        new PageList<AccountTransactionGetDto>(
            source.Items.Select(MapToAccountTransactionGetDto).ToList(),
            source.Total,
            source.Page,
            source.PageSize);
}
