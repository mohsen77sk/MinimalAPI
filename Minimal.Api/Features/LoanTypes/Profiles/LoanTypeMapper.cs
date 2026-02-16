using Riok.Mapperly.Abstractions;
using Minimal.Api.Features.LoanTypes.Models;
using Minimal.Api.Models;
using Minimal.Domain;

namespace Minimal.Api.Features.LoanTypes.Profiles;

[Mapper]
public partial class LoanTypeMapper
{
    [MapperIgnoreSource(nameof(LoanType.Loans))]
    public partial LoanTypeGetDto MapToLoanTypeGetDto(LoanType source);

    [MapperIgnoreSource(nameof(LoanType.Loans))]
    [MapperIgnoreSource(nameof(LoanType.IsActive))]
    public partial LookupDto MapToLookupDto(LoanType source);

    public PageList<LoanTypeGetDto> MapToPageList(PageList<LoanType> source) =>
        new PageList<LoanTypeGetDto>(
            source.Items.Select(MapToLoanTypeGetDto).ToList(),
            source.Total,
            source.Page,
            source.PageSize);
}
