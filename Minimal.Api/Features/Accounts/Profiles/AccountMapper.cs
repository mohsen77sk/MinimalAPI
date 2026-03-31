using Minimal.Api.Features.Accounts.Commands;
using Minimal.Api.Features.Accounts.Models;
using Minimal.Api.Models;
using Minimal.Domain;

namespace Minimal.Api.Features.Accounts.Profiles;

public class AccountMapper
{
    public Account MapToAccount(CreateAccount source) =>
        new Account
        {
            AccountTypeId = source.AccountTypeId,
            CreateDate = source.CreateDate,
            Note = source.Note
        };

    public LookupDto MapToLookupDto(Account source) =>
        new LookupDto
        {
            Id = source.Id,
            Code = source.Code,
            Name = source == null ? string.Empty : (source.AccountType?.Name ?? "") + " - " + string.Join(", ", source.People?.Select(p => p.FullName) ?? [])
        };

    public AccountGetDto MapToAccountGetDto(Account source) =>
        new AccountGetDto
        {
            Id = source.Id,
            Code = source.Code,
            AccountTypeId = source.AccountTypeId,
            AccountTypeName = source.AccountType?.Name ?? string.Empty,
            Persons = source.People?.Select(p => new LookupDto { Id = p.Id, Code = p.Code, Name = p.FullName }).ToList() ?? [],
            CreateDate = source.CreateDate,
            CloseDate = source.CloseDate,
            Note = source.Note,
            IsActive = source.IsActive
        };

    public PageList<AccountGetDto> MapToPageList(PageList<Account> source) =>
        new PageList<AccountGetDto>(
            source.Items.Select(MapToAccountGetDto).ToList(),
            source.Total,
            source.Page,
            source.PageSize);
}
