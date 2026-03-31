using Riok.Mapperly.Abstractions;
using Minimal.Api.Features.LoanTypes.Models;
using Minimal.Api.Models;
using Minimal.Domain;

namespace Minimal.Api.Features.LoanTypes.Profiles;

[Mapper]
public partial class LoanTypeMapper
{
    public LoanTypeGetDto MapToLoanTypeGetDto(LoanType source) =>
        new LoanTypeGetDto
        {
            Id = source.Id,
            Code = source.Code,
            Name = source.Name,
            IsActive = source.IsActive
        };

    public LookupDto MapToLookupDto(LoanType source) =>
        new LookupDto
        {
            Id = source.Id,
            Code = source.Code,
            Name = source.Name
        };

    public PageList<LoanTypeGetDto> MapToPageList(PageList<LoanType> source) =>
        new PageList<LoanTypeGetDto>(
            source.Items.Select(MapToLoanTypeGetDto).ToList(),
            source.Total,
            source.Page,
            source.PageSize);
}
