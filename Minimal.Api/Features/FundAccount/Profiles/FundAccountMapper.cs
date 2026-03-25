using Riok.Mapperly.Abstractions;
using Minimal.Api.Features.FundAccount.Models;
using Minimal.Api.Models;
using Minimal.Domain;

namespace Minimal.Api.Features.FundAccount.Profiles;

public class FundAccountMapper
{
    public FundAccountTransactionGetDto MapToFundAccountTransactionGetDto(DocumentArticle source) =>
        new FundAccountTransactionGetDto
        {
            Id = source.Id,
            Code = source.Document.Code,
            Date = source.Document.Date,
            Note = source.Document.Note,
            Credit = source.Debit,
            Debit = source.Credit,
        };

    public PageList<FundAccountTransactionGetDto> MapToPageList(PageList<DocumentArticle> source) =>
        new PageList<FundAccountTransactionGetDto>(
            source.Items.Select(MapToFundAccountTransactionGetDto).ToList(),
            source.Total,
            source.Page,
            source.PageSize);
}
