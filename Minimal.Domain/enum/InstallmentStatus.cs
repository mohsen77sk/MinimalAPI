namespace Minimal.Domain;

public enum InstallmentStatus : byte
{
    Pending = 0,
    PartiallyPaid = 1,
    Paid = 2,
    Overdue = 3
}