using Microsoft.AspNetCore.Identity;

namespace Minimal.Domain.Identity;

public class ApplicationUser : IdentityUser
{
    public int? PersonId { get; set; }
    public Person? Person { get; set; }
}