namespace Minimal.Domain;

public class LoanType
{
    public LoanType()
    {
        Loans = new List<Loan>();
    }

    public int Id { get; set; }

    public string Code { get; set; } = default!;

    public string Name { get; set; } = default!;

    public bool IsActive { get; set; }

    public ICollection<Loan> Loans { get; set; }
}