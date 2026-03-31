namespace Minimal.Domain;

public class LoanPaymentAllocation
{
    public int Id { get; set; }

    public int PaymentId { get; set; }
    public LoanPayment Payment { get; set; } = default!;

    public int InstallmentId { get; set; }
    public LoanInstallment Installment { get; set; } = default!;

    public decimal Amount { get; set; }
}