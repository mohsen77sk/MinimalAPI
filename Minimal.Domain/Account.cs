namespace Minimal.Domain;

public class Account
{
    public Account()
    {
        People = new List<Person>();
        Loans = new List<Loan>();
    }

    public int Id { get; set; }

    public string Code { get; set; } = default!;

    public int AccountTypeId { get; set; }
    public AccountType AccountType { get; set; } = default!;

    public DateTimeOffset CreateDate { get; set; }

    public DateTimeOffset? CloseDate { get; set; }

    public string Note { get; set; } = default!;

    public bool IsActive { get; set; }

    public AccountDetail AccountDetail { get; set; } = default!;

    public ICollection<Person> People { get; set; }

    public ICollection<Loan> Loans { get; set; }
}