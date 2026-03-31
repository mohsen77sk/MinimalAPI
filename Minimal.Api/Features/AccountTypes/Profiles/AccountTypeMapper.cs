using Minimal.Api.Features.AccountTypes.Models;
using Minimal.Api.Models;
using Minimal.Domain;

namespace Minimal.Api.Features.AccountTypes.Profiles;

public class AccountTypeMapper
{
    public AccountTypeGetDto MapToAccountTypeGetDto(AccountType source) =>
        new AccountTypeGetDto
        {
            Id = source.Id,
            Code = source.Code,
            Name = source.Name,
            IsActive = source.IsActive
        };

    public LookupDto MapToLookupDto(AccountType source) =>
        new LookupDto
        {
            Id = source.Id,
            Code = source.Code,
            Name = source.Name
        };

    public PageList<AccountTypeGetDto> MapToPageList(PageList<AccountType> source) =>
        new PageList<AccountTypeGetDto>(
            source.Items.Select(MapToAccountTypeGetDto).ToList(),
            source.Total,
            source.Page,
            source.PageSize);
}
