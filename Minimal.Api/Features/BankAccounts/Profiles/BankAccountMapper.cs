using Minimal.Api.Features.BankAccounts.Commands;
using Minimal.Api.Features.BankAccounts.Models;
using Minimal.Api.Models;
using Minimal.Domain;

namespace Minimal.Api.Features.BankAccounts.Profiles;

public class BankAccountMapper
{
    public BankAccount MapToBankAccount(CreateBankAccount source) =>
        new BankAccount
        {
            PersonId = source.PersonId,
            BankId = source.BankId,
            BranchCode = source.BranchCode,
            BranchName = source.BranchName,
            AccountNumber = source.AccountNumber,
            CardNumber = source.CardNumber,
            Iban = source.Iban
        };

    public BankAccountGetDto MapToBankAccountGetDto(BankAccount source) =>
        new BankAccountGetDto
        {
            Id = source.Id,
            PersonId = source.PersonId,
            PersonName = source.Person?.FullName ?? string.Empty,
            BankId = source.BankId,
            BankName = source.Bank?.Name ?? string.Empty,
            BranchCode = source.BranchCode,
            BranchName = source.BranchName,
            AccountNumber = source.AccountNumber,
            CardNumber = source.CardNumber,
            Iban = source.Iban,
            IsActive = source.IsActive
        };

    public PageList<BankAccountGetDto> MapToPageList(PageList<BankAccount> source) =>
        new PageList<BankAccountGetDto>(
            source.Items.Select(MapToBankAccountGetDto).ToList(),
            source.Total,
            source.Page,
            source.PageSize);
}
