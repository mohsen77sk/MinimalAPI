using Riok.Mapperly.Abstractions;
using Minimal.Api.Features.Accounts.Commands;
using Minimal.Api.Features.Accounts.Models;
using Minimal.Api.Models;
using Minimal.Domain;

namespace Minimal.Api.Features.Accounts.Profiles;

[Mapper]
public partial class AccountMapper
{
    [MapperIgnoreSource(nameof(CreateAccount.InitCredit))]
    [MapperIgnoreSource(nameof(CreateAccount.PersonId))]
    [MapperIgnoreTarget(nameof(Account.Id))]
    [MapperIgnoreTarget(nameof(Account.Code))]
    [MapperIgnoreTarget(nameof(Account.AccountType))]
    [MapperIgnoreTarget(nameof(Account.CloseDate))]
    [MapperIgnoreTarget(nameof(Account.IsActive))]
    [MapperIgnoreTarget(nameof(Account.AccountDetail))]
    [MapperIgnoreTarget(nameof(Account.People))]
    [MapperIgnoreTarget(nameof(Account.Loans))]
    public partial Account MapToAccount(CreateAccount source);

    [MapProperty(nameof(Account), nameof(LookupDto.Name), Use = nameof(GetAccountLookupName))]
    public partial LookupDto MapToLookupDto(Account source);

    [MapProperty(nameof(Account.AccountType), nameof(AccountGetDto.AccountTypeName), Use = nameof(GetAccountTypeName))]
    [MapProperty(nameof(Account.People), nameof(AccountGetDto.Persons), Use = nameof(MapPeopleToLookup))]
    [MapperIgnoreSource(nameof(Account.AccountDetail))]
    [MapperIgnoreSource(nameof(Account.Loans))]
    public partial AccountGetDto MapToAccountGetDto(Account source);

    public PageList<AccountGetDto> MapToPageList(PageList<Account> source) =>
        new PageList<AccountGetDto>(
            source.Items.Select(MapToAccountGetDto).ToList(),
            source.Total,
            source.Page,
            source.PageSize);

    private static string GetAccountLookupName(Account? account) =>
        account == null ? string.Empty
        : (account.AccountType?.Name ?? "") + " - " + string.Join(", ", account.People?.Select(p => p.FullName) ?? []);

    private static string GetAccountTypeName(AccountType? at) => at?.Name ?? string.Empty;

    private static IList<LookupDto> MapPeopleToLookup(ICollection<Person>? people) =>
        people?.Select(p => new LookupDto { Id = p.Id, Code = p.Code, Name = p.FullName }).ToList() ?? [];
}
