using Riok.Mapperly.Abstractions;
using Minimal.Api.Features.BankAccounts.Commands;
using Minimal.Api.Features.BankAccounts.Models;
using Minimal.Api.Models;
using Minimal.Domain;

namespace Minimal.Api.Features.BankAccounts.Profiles;

[Mapper]
public partial class BankAccountMapper
{
    [MapperIgnoreTarget(nameof(BankAccount.Id))]
    [MapperIgnoreTarget(nameof(BankAccount.Bank))]
    [MapperIgnoreTarget(nameof(BankAccount.IsActive))]
    [MapperIgnoreTarget(nameof(BankAccount.Person))]
    public partial BankAccount MapToBankAccount(CreateBankAccount source);

    [MapProperty(nameof(BankAccount.Person), nameof(BankAccountGetDto.PersonName), Use = nameof(GetPersonFullName))]
    [MapProperty(nameof(BankAccount.Bank), nameof(BankAccountGetDto.BankName), Use = nameof(GetBankName))]
    public partial BankAccountGetDto MapToBankAccountGetDto(BankAccount source);

    public PageList<BankAccountGetDto> MapToPageList(PageList<BankAccount> source) =>
        new PageList<BankAccountGetDto>(
            source.Items.Select(MapToBankAccountGetDto).ToList(),
            source.Total,
            source.Page,
            source.PageSize);

    private static string GetPersonFullName(Person? person) => person?.FullName ?? string.Empty;
    private static string GetBankName(Bank? bank) => bank?.Name ?? string.Empty;
}
