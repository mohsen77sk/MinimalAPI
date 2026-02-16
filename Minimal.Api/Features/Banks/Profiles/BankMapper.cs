using Riok.Mapperly.Abstractions;
using Minimal.Api.Features.Banks.Models;
using Minimal.Api.Models;
using Minimal.Domain;

namespace Minimal.Api.Features.Banks.Profiles;

[Mapper]
public partial class BankMapper
{
    [MapperIgnoreSource(nameof(Bank.BankAccounts))]
    public partial BankGetDto MapToBankGetDto(Bank source);

    [MapperIgnoreSource(nameof(Bank.BankAccounts))]
    [MapperIgnoreSource(nameof(Bank.IsActive))]
    public partial LookupDto MapToLookupDto(Bank source);

    public PageList<BankGetDto> MapToPageList(PageList<Bank> source) =>
        new PageList<BankGetDto>(
            source.Items.Select(MapToBankGetDto).ToList(),
            source.Total,
            source.Page,
            source.PageSize);
}
