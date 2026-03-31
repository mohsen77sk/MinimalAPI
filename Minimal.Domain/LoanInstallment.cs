namespace Minimal.Domain;

public class LoanInstallment
{
    public LoanInstallment()
    {
        PaymentAllocations = [];
    }

    public int Id { get; set; }

    public int LoanId { get; set; }
    public Loan Loan { get; set; } = default!;

    public int Number { get; set; }

    public DateTimeOffset DueDate { get; set; }

    public DateTimeOffset? PaidDate { get; set; }

    public decimal PrincipalAmount { get; set; }

    public decimal InterestAmount { get; set; }

    public decimal Amount => PrincipalAmount + InterestAmount;

    public decimal PaidPrincipal { get; set; }

    public decimal PaidInterest { get; set; }

    public decimal PaidAmount => PaidPrincipal + PaidInterest;

    public InstallmentStatus Status { get; set; }

    public ICollection<LoanPaymentAllocation> PaymentAllocations { get; set; }
}