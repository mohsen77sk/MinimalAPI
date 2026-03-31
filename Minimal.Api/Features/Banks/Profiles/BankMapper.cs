using Minimal.Api.Features.Banks.Models;
using Minimal.Api.Models;
using Minimal.Domain;

namespace Minimal.Api.Features.Banks.Profiles;

public partial class BankMapper
{
    public BankGetDto MapToBankGetDto(Bank source) =>
        new BankGetDto
        {
            Id = source.Id,
            Code = source.Code,
            Name = source.Name,
            IsActive = source.IsActive
        };

    public LookupDto MapToLookupDto(Bank source) =>
        new LookupDto
        {
            Id = source.Id,
            Code = source.Code,
            Name = source.Name
        };

    public PageList<BankGetDto> MapToPageList(PageList<Bank> source) =>
        new PageList<BankGetDto>(
            source.Items.Select(MapToBankGetDto).ToList(),
            source.Total,
            source.Page,
            source.PageSize);
}
