using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Minimal.Api.Extensions;
using Minimal.DataAccess;
using Minimal.Domain.Identity;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddApplicationServices(builder);

var app = builder.Build();
app.ConfigureApplication();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    if (context.Database.IsSqlServer())
    {
        context.Database.Migrate();
    }

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    await ApplicationDbContextSeed.SeedDefaultRolesAndUserAsync(userManager, roleManager);
}

app.Run();
