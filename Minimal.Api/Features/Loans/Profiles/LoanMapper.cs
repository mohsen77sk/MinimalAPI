using Riok.Mapperly.Abstractions;
using Minimal.Api.Features.Loans.Commands;
using Minimal.Api.Features.Loans.Models;
using Minimal.Api.Models;
using Minimal.Domain;

namespace Minimal.Api.Features.Loans.Profiles;

[Mapper(IgnoreObsoleteMembersStrategy = IgnoreObsoleteMembersStrategy.Both)]
public partial class LoanMapper
{
    public partial Loan MapToLoan(CreateLoan source);
    
    [MapProperty(nameof(Loan.Account), nameof(LoanGetDto.AccountCode), Use = nameof(GetAccountCode))]
    [MapProperty(nameof(Loan.LoanType), nameof(LoanGetDto.LoanTypeName), Use = nameof(GetLoanTypeName))]
    public partial LoanGetDto MapToLoanGetDto(Loan source);

    public PageList<LoanGetDto> MapToPageList(PageList<Loan> source) =>
        new PageList<LoanGetDto>(
            source.Items.Select(MapToLoanGetDto).ToList(),
            source.Total,
            source.Page,
            source.PageSize);

    private static string GetAccountCode(Account? account) => account?.Code ?? string.Empty;
    private static string GetLoanTypeName(LoanType? loanType) => loanType?.Name ?? string.Empty;
}
