using Minimal.Domain.Identity;

namespace Minimal.Domain;

public class Person
{
    public Person()
    {
        BankAccounts = [];
        Accounts = [];
    }

    public int Id { get; set; }

    public string Code { get; set; } = default!;

    public string FirstName { get; set; } = default!;

    public string LastName { get; set; } = default!;

    public string FullName => FirstName + " " + LastName;

    public string NationalCode { get; set; } = default!;

    public DateTimeOffset? Birthday { get; set; }

    public byte Gender { get; set; }

    public string? Note { get; set; }

    public bool IsActive { get; set; }

    public ApplicationUser? User { get; set; }

    public ICollection<BankAccount> BankAccounts { get; set; }

    public ICollection<Account> Accounts { get; set; }
}