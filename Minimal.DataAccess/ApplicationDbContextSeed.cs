using Microsoft.AspNetCore.Identity;
using Minimal.Domain;
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

    public static async Task SeedDataAsync(ApplicationDbContext context)
    {
        if (!context.Banks.Any())
        {
            context.AddRange(new List<Bank>() {
                new Bank { Code = "010", Name = "بانک مرکزی جمهوری اسلامی", IsActive = true },
                new Bank { Code = "011", Name = "بانک صنعت و معدن", IsActive = true },
                new Bank { Code = "012", Name = "بانک ملت", IsActive = true },
                new Bank { Code = "013", Name = "بانک رفاه کارگران", IsActive = true },
                new Bank { Code = "014", Name = "بانک مسکن", IsActive = true },
                new Bank { Code = "015", Name = "بانک سپه", IsActive = true },
                new Bank { Code = "016", Name = "بانک کشاورزی", IsActive = true },
                new Bank { Code = "017", Name = "بانک ملی ایران", IsActive = true },
                new Bank { Code = "018", Name = "بانک تجارت", IsActive = true },
                new Bank { Code = "019", Name = "بانک صادرات ایران", IsActive = true },
                new Bank { Code = "020", Name = "بانک توسعه صادرات ایران", IsActive = true },
                new Bank { Code = "021", Name = "پست بانک ایران", IsActive = true },
                new Bank { Code = "022", Name = "بانک توسعه تعاون", IsActive = true },
                new Bank { Code = "051", Name = "مؤسسه اعتباری غیربانکی توسعه", IsActive = true },
                new Bank { Code = "053", Name = "بانک کارآفرین", IsActive = true },
                new Bank { Code = "054", Name = "بانک پارسیان", IsActive = true },
                new Bank { Code = "055", Name = "بانک اقتصادنوین", IsActive = true },
                new Bank { Code = "056", Name = "بانک سامان", IsActive = true },
                new Bank { Code = "057", Name = "بانک پاسارگاد", IsActive = true },
                new Bank { Code = "058", Name = "بانک سرمایه", IsActive = true },
                new Bank { Code = "059", Name = "بانک سینا", IsActive = true },
                new Bank { Code = "060", Name = "بانک قرض‌الحسنه مهر ایران", IsActive = true },
                new Bank { Code = "061", Name = "بانک شهر", IsActive = true },
                new Bank { Code = "062", Name = "بانک آینده", IsActive = true },
                new Bank { Code = "064", Name = "بانک گردشگری", IsActive = true },
                new Bank { Code = "066", Name = "بانک دی", IsActive = true },
                new Bank { Code = "069", Name = "بانک ایران زمین", IsActive = true },
                new Bank { Code = "070", Name = "بانک قرض‌الحسنه رسالت", IsActive = true },
                new Bank { Code = "075", Name = "مؤسسه اعتباری غیربانکی ملل", IsActive = true },
                new Bank { Code = "078", Name = "بانک خاورمیانه", IsActive = true },
                new Bank { Code = "095", Name = "بانک مشترک ایران - ونزوئلا", IsActive = true },
                new Bank { Code = "000", Name = "مؤسسه اعتباری غیربانکی كاسپین", IsActive = true },
                new Bank { Code = "000", Name = "موسسه اعتباری غیربانکی نور", IsActive = true },
            });
            await context.SaveChangesAsync();
        }
    }
}