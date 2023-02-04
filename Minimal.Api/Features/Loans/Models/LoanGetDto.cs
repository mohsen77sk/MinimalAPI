using Minimal.Api.Models;

namespace Minimal.Api.Features.Loans.Models;

public class LoanGetDto
{
    public int Id { get; init; }

    public string Code { get; set; } = default!;

    public int AccountId { get; set; }

    public string AccountCode { get; set; } = default!;

    public int LoanTypeId { get; set; }

    public string LoanTypeName { get; set; } = default!;

    public DateTimeOffset CreateDate { get; set; }

    public DateTimeOffset? CloseDate { get; set; }

    public decimal Amount { get; set; }

    public decimal InstallmentAmount { get; set; }

    public int InstallmentCount { get; set; }

    public int InstallmentInterval { get; set; }

    public int InterestRates { get; set; }

    public string Note { get; set; } = default!;

    public bool IsActive { get; set; }
}