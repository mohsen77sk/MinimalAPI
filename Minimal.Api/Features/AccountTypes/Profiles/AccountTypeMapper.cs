using Riok.Mapperly.Abstractions;
using Minimal.Api.Features.AccountTypes.Models;
using Minimal.Api.Models;
using Minimal.Domain;

namespace Minimal.Api.Features.AccountTypes.Profiles;

[Mapper(IgnoreObsoleteMembersStrategy = IgnoreObsoleteMembersStrategy.Both)]
public partial class AccountTypeMapper
{
    public partial AccountTypeGetDto MapToAccountTypeGetDto(AccountType source);
    
    public partial LookupDto MapToLookupDto(AccountType source);

    public PageList<AccountTypeGetDto> MapToPageList(PageList<AccountType> source) =>
        new PageList<AccountTypeGetDto>(
            source.Items.Select(MapToAccountTypeGetDto).ToList(),
            source.Total,
            source.Page,
            source.PageSize);
}
