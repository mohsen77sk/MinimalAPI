namespace Minimal.Domain;

public class Loan
{
    public int Id { get; set; }

    public string Code { get; set; } = default!;

    public int LoanTypeId { get; set; }
    public LoanType LoanType { get; set; } = default!;

    public int AccountId { get; set; }
    public Account Account { get; set; } = default!;

    public DateTimeOffset CreateDate { get; set; }

    public DateTimeOffset? CloseDate { get; set; }

    public decimal Amount { get; set; }

    public decimal InstallmentAmount { get; set; }

    public DateTimeOffset StartInstallmentPayment { get; set; }

    public int InstallmentCount { get; set; }

    public int InstallmentInterval { get; set; }

    public int InterestRates { get; set; }

    public string Note { get; set; } = default!;

    public bool IsActive { get; set; }

    public AccountDetail AccountDetail { get; set; } = default!;
}