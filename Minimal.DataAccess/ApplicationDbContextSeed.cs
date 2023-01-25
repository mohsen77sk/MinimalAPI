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

    public static async Task SeedAccountingDataAsync(ApplicationDbContext context)
    {
        if (!context.AccountCategories.Any())
        {
            context.AddRange(new List<AccountCategory>()
            {
                new AccountCategory { Code = "1", Title = "صندوق" },
                new AccountCategory { Code = "2", Title = "حساب" },
                new AccountCategory { Code = "3", Title = "تسهیلات" },
            });
            await context.SaveChangesAsync();
        }

        if (!context.AccountEssences.Any())
        {
            context.AddRange(new List<AccountEssence>()
            {
                new AccountEssence { Code = "1", Title = "ماهیت بدهکار" },
                new AccountEssence { Code = "2", Title = "ماهیت بستانکار" },
                new AccountEssence { Code = "3", Title = "ماهیت دوگانه" },
                new AccountEssence { Code = "4", Title = "فاقد ماهیت" },
            });
            await context.SaveChangesAsync();
        }

        if (!context.AccountGroups.Any())
        {
            context.AddRange(new List<AccountGroup>()
            {
                new AccountGroup { Code = "1", Title = "دارایی ها", IsSystemic = true },
                new AccountGroup { Code = "2", Title = "بدهی ها", IsSystemic = true },
                new AccountGroup { Code = "3", Title = "درآمد ها", IsSystemic = true },
                new AccountGroup { Code = "4", Title = "هزینه ها", IsSystemic = true },
                new AccountGroup { Code = "5", Title = "سایر حساب ها", IsSystemic = true },
            });
            await context.SaveChangesAsync();
        }

        if (!context.AccountLedgers.Any())
        {
            context.AddRange(new List<AccountLedger>()
            {
                new AccountLedger { Code = "11", AccountGroup = context.AccountGroups.Single(x => x.Code == "1"), Title = "موجودی نقد و بانک", IsSystemic = true },
                new AccountLedger { Code = "12", AccountGroup = context.AccountGroups.Single(x => x.Code == "1"), Title = "تسهیلات اعطایی", IsSystemic = true },
                new AccountLedger { Code = "21", AccountGroup = context.AccountGroups.Single(x => x.Code == "2"), Title = "حساب قرض الحسنه", IsSystemic = true },
                new AccountLedger { Code = "22", AccountGroup = context.AccountGroups.Single(x => x.Code == "2"), Title = "حساب سرمایه گذاری", IsSystemic = true },
                new AccountLedger { Code = "31", AccountGroup = context.AccountGroups.Single(x => x.Code == "3"), Title = "درآمد حاصل از اراعه خدمات", IsSystemic = true },
                new AccountLedger { Code = "32", AccountGroup = context.AccountGroups.Single(x => x.Code == "3"), Title = "درآمد های متفرقه", IsSystemic = true },
                new AccountLedger { Code = "41", AccountGroup = context.AccountGroups.Single(x => x.Code == "4"), Title = "هزینه های عملیاتی", IsSystemic = true },
                new AccountLedger { Code = "42", AccountGroup = context.AccountGroups.Single(x => x.Code == "4"), Title = "هزینه های متفرقه", IsSystemic = true },
            });
            await context.SaveChangesAsync();
        }

        if (!context.AccountSubsids.Any())
        {
            context.AddRange(new List<AccountSubsid>()
            {
                new AccountSubsid { Code = "1101", AccountLedger = context.AccountLedgers.Single(x => x.Code == "11"), Title = "صندوق", AccountEssence = context.AccountEssences.Single(x => x.Code == "1"), IsSystemic = true },
                new AccountSubsid { Code = "1102", AccountLedger = context.AccountLedgers.Single(x => x.Code == "11"), Title = "بانک ها", AccountEssence = context.AccountEssences.Single(x => x.Code == "1"), IsSystemic = true },
                new AccountSubsid { Code = "1103", AccountLedger = context.AccountLedgers.Single(x => x.Code == "11"), Title = "تنخواه گردان ها", AccountEssence = context.AccountEssences.Single(x => x.Code == "1"), IsSystemic = true },
                new AccountSubsid { Code = "1201", AccountLedger = context.AccountLedgers.Single(x => x.Code == "12"), Title = "قرض الحسنه", AccountEssence = context.AccountEssences.Single(x => x.Code == "1"), IsSystemic = true },
                new AccountSubsid { Code = "1202", AccountLedger = context.AccountLedgers.Single(x => x.Code == "12"), Title = "اضطراری", AccountEssence = context.AccountEssences.Single(x => x.Code == "1"), IsSystemic = true },
                new AccountSubsid { Code = "2101", AccountLedger = context.AccountLedgers.Single(x => x.Code == "21"), Title = "قرض الحسنه پس انداز", AccountEssence = context.AccountEssences.Single(x => x.Code == "2"), IsSystemic = true },
                new AccountSubsid { Code = "2102", AccountLedger = context.AccountLedgers.Single(x => x.Code == "21"), Title = "قرض الحسنه جاری", AccountEssence = context.AccountEssences.Single(x => x.Code == "2"), IsSystemic = true },
                new AccountSubsid { Code = "2201", AccountLedger = context.AccountLedgers.Single(x => x.Code == "22"), Title = "سرمایه گذاری کوتاه مدت", AccountEssence = context.AccountEssences.Single(x => x.Code == "2"), IsSystemic = true },
                new AccountSubsid { Code = "2202", AccountLedger = context.AccountLedgers.Single(x => x.Code == "22"), Title = "سرمایه گذاری بلند مدت", AccountEssence = context.AccountEssences.Single(x => x.Code == "2"), IsSystemic = true },
                new AccountSubsid { Code = "3101", AccountLedger = context.AccountLedgers.Single(x => x.Code == "31"), Title = "سود حاصل از تسهیلات", AccountEssence = context.AccountEssences.Single(x => x.Code == "2"), IsSystemic = true },
                new AccountSubsid { Code = "3102", AccountLedger = context.AccountLedgers.Single(x => x.Code == "31"), Title = "کارمزد", AccountEssence = context.AccountEssences.Single(x => x.Code == "2"), IsSystemic = true },
                new AccountSubsid { Code = "4101", AccountLedger = context.AccountLedgers.Single(x => x.Code == "41"), Title = "ملزومات", AccountEssence = context.AccountEssences.Single(x => x.Code == "1"), IsSystemic = true },
            });
            await context.SaveChangesAsync();
        }

        if (!context.AccountDetails.Any())
        {
            context.AddRange(new List<AccountDetail>()
            {
                new AccountDetail { Code = "11010001", Title = "حساب صندوق", AccountCategory = context.AccountCategories.Single(x => x.Code == "1") }
            });
            await context.SaveChangesAsync();
        }

        if (!context.DocumentTypes.Any())
        {
            context.AddRange(new List<DocumentType>()
            {
                new DocumentType { Code = "01", Title = "سند حسابداری" },
                //
                new DocumentType { Code = "10", Title = "سند افتتاحیه" },
                new DocumentType { Code = "11", Title = "سند اختتامیه" },
                new DocumentType { Code = "12", Title = "سند دریافت" },
                new DocumentType { Code = "13", Title = "سند پرداخت" },
                new DocumentType { Code = "14", Title = "سند انتقال" },
                //
                new DocumentType { Code = "20", Title = "سند پرداخت تسهیلات" },
                new DocumentType { Code = "21", Title = "سند دریافت اقساط" },
                //
                new DocumentType { Code = "30", Title = "سند صورت هزینه" },
            });
            await context.SaveChangesAsync();
        }

        if (!context.FiscalYears.Any())
        {
            context.AddRange(new List<FiscalYear>()
            {
                new FiscalYear { Title = "", BeginDate = DateTimeOffset.MinValue , EndDate = DateTimeOffset.MaxValue }
            });
            await context.SaveChangesAsync();
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

        if (!context.AccountTypes.Any())
        {
            context.AddRange(new List<AccountType>()
            {
                new AccountType { Code = "2101", Name = "قرض الحسنه پس انداز", IsActive = true },
                new AccountType { Code = "2102", Name = "قرض الحسنه جاری", IsActive = true },
                new AccountType { Code = "2201", Name = "سرمایه گذاری کوتاه مدت", IsActive = true },
                new AccountType { Code = "2202", Name = "سرمایه گذاری بلند مدت", IsActive = true },
            });
            await context.SaveChangesAsync();
        }
    }
}