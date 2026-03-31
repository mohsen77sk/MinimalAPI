namespace Minimal.Domain;

public class LoanPayment
{
    public LoanPayment()
    {
        Allocations = [];
    }

    public int Id { get; set; }

    public int LoanId { get; set; }
    public Loan Loan { get; set; } = default!;

    public decimal Amount { get; set; }

    public DateTimeOffset PaymentDate { get; set; }

    public int DocumentId { get; set; }
    public Document Document { get; set; } = default!;

    public string Note { get; set; } = default!;

    public ICollection<LoanPaymentAllocation> Allocations { get; set; }
}