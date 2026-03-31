using Minimal.Api.Features.Loans.Models;
using Minimal.Api.Models;
using Minimal.Domain;

namespace Minimal.Api.Features.Loans.Profiles;

public class LoanMapper
{
    public LoanGetDto MapToLoanGetDto(Loan source) =>
        new LoanGetDto
        {
            Id = source.Id,
            Code = source.Code,
            AccountId = source.AccountId,
            AccountCode = source.Account?.Code ?? string.Empty,
            LoanTypeId = source.LoanTypeId,
            LoanTypeName = source.LoanType?.Name ?? string.Empty,
            CreateDate = source.CreateDate,
            CloseDate = source.CloseDate,
            Amount = source.Amount,
            InstallmentAmount = source.InstallmentAmount,
            InstallmentCount = source.InstallmentCount,
            InstallmentInterval = source.InstallmentInterval,
            InterestRates = source.InterestRates,
            IsActive = source.IsActive,
            Note = source.Note
        };

    public PageList<LoanGetDto> MapToPageList(PageList<Loan> source) =>
        new PageList<LoanGetDto>(
            source.Items.Select(MapToLoanGetDto).ToList(),
            source.Total,
            source.Page,
            source.PageSize);

    public LoanInstallmentsGetDto MapToLoanInstalmentGetDto(LoanInstallment source) =>
        new LoanInstallmentsGetDto
        {
            Id = source.Id,
            Number = source.Number,
            DueDate = source.DueDate,
            PaidDate = source.PaidDate,
            Amount = source.Amount,
            PaidAmount = source.PaidAmount,
            Status = source.Status
        };

    public LoanPaymentsGetDto MapToLoanPaymentGetDto(LoanPayment source) =>
        new LoanPaymentsGetDto
        {
            Id = source.Id,
            Amount = source.Amount,
            PaymentDate = source.PaymentDate,
            DocumentId = source.DocumentId,
            DocumentCode = source.Document.Code,
            Note = source.Note
        };
}