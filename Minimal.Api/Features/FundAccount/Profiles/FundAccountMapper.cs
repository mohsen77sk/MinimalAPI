using Riok.Mapperly.Abstractions;
using Minimal.Api.Features.FundAccount.Models;
using Minimal.Api.Models;
using Minimal.Domain;

namespace Minimal.Api.Features.FundAccount.Profiles;

[Mapper(IgnoreObsoleteMembersStrategy = IgnoreObsoleteMembersStrategy.Both)]
public partial class FundAccountMapper
{
    [MapProperty(nameof(DocumentArticle.Document), nameof(FundAccountTransactionGetDto.Date), Use = nameof(GetDocumentDate))]
    [MapProperty(nameof(DocumentArticle.Debit), nameof(FundAccountTransactionGetDto.Credit))]
    [MapProperty(nameof(DocumentArticle.Credit), nameof(FundAccountTransactionGetDto.debit))]
    public partial FundAccountTransactionGetDto MapToFundAccountTransactionGetDto(DocumentArticle source);

    public PageList<FundAccountTransactionGetDto> MapToPageList(PageList<DocumentArticle> source) =>
        new PageList<FundAccountTransactionGetDto>(
            source.Items.Select(MapToFundAccountTransactionGetDto).ToList(),
            source.Total,
            source.Page,
            source.PageSize);

    private static DateTimeOffset GetDocumentDate(Document? doc) => doc?.Date ?? default;
}
