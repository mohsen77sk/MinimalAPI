using Riok.Mapperly.Abstractions;
using Minimal.Api.Features.AccountTypes.Models;
using Minimal.Api.Models;
using Minimal.Domain;

namespace Minimal.Api.Features.AccountTypes.Profiles;

[Mapper]
public partial class AccountTypeMapper
{
    [MapperIgnoreSource(nameof(AccountType.Accounts))]
    public partial AccountTypeGetDto MapToAccountTypeGetDto(AccountType source);

    [MapperIgnoreSource(nameof(AccountType.Accounts))]
    [MapperIgnoreSource(nameof(AccountType.IsActive))]
    public partial LookupDto MapToLookupDto(AccountType source);

    public PageList<AccountTypeGetDto> MapToPageList(PageList<AccountType> source) =>
        new PageList<AccountTypeGetDto>(
            source.Items.Select(MapToAccountTypeGetDto).ToList(),
            source.Total,
            source.Page,
            source.PageSize);
}
