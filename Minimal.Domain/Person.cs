using Minimal.Domain.Identity;

namespace Minimal.Domain;

public class Person
{
    public Person()
    {
        BankAccounts = new List<BankAccount>();
    }

    public int Id { get; set; }

    public string Code { get; set; } = default!;

    public string FirstName { get; set; } = default!;

    public string LastName { get; set; } = default!;

    public string FullName => FirstName + " " + LastName;

    public string NationalCode { get; set; } = default!;

    public DateTime? DateOfBirth { get; set; }

    public byte Gender { get; set; }

    public string? Note { get; set; }

    public bool IsActive { get; set; }

    public ApplicationUser? User { get; set; }

    public ICollection<BankAccount> BankAccounts { get; set; }
}