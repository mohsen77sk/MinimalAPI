using Minimal.Api.Features.LoanTransactions.Models;
using Minimal.Api.Models;
using Minimal.Domain;

namespace Minimal.Api.Features.LoanTransactions.Profiles;

public class LoanTransactionMapper
{
    public LoanTransactionGetDto MapToLoanTransactionGetDto(Document source) =>
        new LoanTransactionGetDto
        {
            Id = source.Id,
            Code = source.Code,
            Date = source.Date,
            Note = source.Note ?? string.Empty,
            Amount = GetAmount(source),
            Editable = GetEditable(source)
        };

    public PageList<LoanTransactionGetDto> MapToPageList(PageList<Document> source) =>
        new PageList<LoanTransactionGetDto>(
            source.Items.Select(MapToLoanTransactionGetDto).ToList(),
            source.Total,
            source.Page,
            source.PageSize);

    private static decimal GetAmount(Document src) =>
        src.DocumentItems?.Where(x => x.AccountDetailId != null).Sum(x => x.Credit + x.Debit) ?? 0;

    private static bool GetEditable(Document src) =>
        src.DocumentType?.Code == "21" && src.DocumentItems?.Any(x => x.AccountDetail?.IsActive == false) != true;
}
