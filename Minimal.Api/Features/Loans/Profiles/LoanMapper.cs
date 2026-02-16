using Riok.Mapperly.Abstractions;
using Minimal.Api.Features.Loans.Commands;
using Minimal.Api.Features.Loans.Models;
using Minimal.Api.Models;
using Minimal.Domain;

namespace Minimal.Api.Features.Loans.Profiles;

[Mapper]
public partial class LoanMapper
{
    [MapperIgnoreTarget(nameof(Loan.Id))]
    [MapperIgnoreTarget(nameof(Loan.Code))]
    [MapperIgnoreTarget(nameof(Loan.LoanType))]
    [MapperIgnoreTarget(nameof(Loan.Account))]
    [MapperIgnoreTarget(nameof(Loan.CloseDate))]
    [MapperIgnoreTarget(nameof(Loan.InstallmentAmount))]
    [MapperIgnoreTarget(nameof(Loan.StartInstallmentPayment))]
    [MapperIgnoreTarget(nameof(Loan.IsActive))]
    [MapperIgnoreTarget(nameof(Loan.AccountDetail))]
    public partial Loan MapToLoan(CreateLoan source);

    [MapProperty(nameof(Loan.Account), nameof(LoanGetDto.AccountCode), Use = nameof(GetAccountCode))]
    [MapProperty(nameof(Loan.LoanType), nameof(LoanGetDto.LoanTypeName), Use = nameof(GetLoanTypeName))]
    [MapperIgnoreSource(nameof(Loan.StartInstallmentPayment))]
    [MapperIgnoreSource(nameof(Loan.AccountDetail))]
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
