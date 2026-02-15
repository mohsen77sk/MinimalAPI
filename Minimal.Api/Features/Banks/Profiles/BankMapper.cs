using Riok.Mapperly.Abstractions;
using Minimal.Api.Features.Banks.Models;
using Minimal.Api.Models;
using Minimal.Domain;

namespace Minimal.Api.Features.Banks.Profiles;

[Mapper(IgnoreObsoleteMembersStrategy = IgnoreObsoleteMembersStrategy.Both)]
public partial class BankMapper
{
    public partial BankGetDto MapToBankGetDto(Bank source);

    public partial LookupDto MapToLookupDto(Bank source);

    public PageList<BankGetDto> MapToPageList(PageList<Bank> source) =>
        new PageList<BankGetDto>(
            source.Items.Select(MapToBankGetDto).ToList(),
            source.Total,
            source.Page,
            source.PageSize);
}
