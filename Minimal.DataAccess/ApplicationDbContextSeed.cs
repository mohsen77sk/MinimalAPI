using Microsoft.AspNetCore.Identity;
using Minimal.Domain.Identity;

namespace Minimal.DataAccess;

public static class ApplicationDbContextSeed
{
    public static async Task SeedDefaultRolesAndUserAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        var administratorRole = new IdentityRole("Administrator");

        if (roleManager.Roles.All(r => r.Name != administratorRole.Name))
        {
            await roleManager.CreateAsync(administratorRole);
        }

        var managerRole = new IdentityRole("Manager");

        if (roleManager.Roles.All(r => r.Name != managerRole.Name))
        {
            await roleManager.CreateAsync(managerRole);
        }

        var memberRole = new IdentityRole("Member");

        if (roleManager.Roles.All(r => r.Name != memberRole.Name))
        {
            await roleManager.CreateAsync(memberRole);
        }

        var administrator = new ApplicationUser { UserName = "administrator", Email = "administrator@minimal-api.ir" };

        if (userManager.Users.All(u => u.UserName != administrator.UserName))
        {
            await userManager.CreateAsync(administrator, "123456@Pass");
            await userManager.AddToRolesAsync(administrator, new string[] { administratorRole.Name });
        }
    }
}