using Riok.Mapperly.Abstractions;
using Minimal.Api.Features.LoanTypes.Models;
using Minimal.Api.Models;
using Minimal.Domain;

namespace Minimal.Api.Features.LoanTypes.Profiles;

[Mapper(IgnoreObsoleteMembersStrategy = IgnoreObsoleteMembersStrategy.Both)]
public partial class LoanTypeMapper
{
    public partial LoanTypeGetDto MapToLoanTypeGetDto(LoanType source);
    
    public partial LookupDto MapToLookupDto(LoanType source);

    public PageList<LoanTypeGetDto> MapToPageList(PageList<LoanType> source) =>
        new PageList<LoanTypeGetDto>(
            source.Items.Select(MapToLoanTypeGetDto).ToList(),
            source.Total,
            source.Page,
            source.PageSize);
}
