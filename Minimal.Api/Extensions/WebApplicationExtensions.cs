using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Minimal.Api.Contracts;
using Minimal.Api.Middleware;
using Minimal.DataAccess;
using Minimal.Domain.Identity;

namespace Minimal.Api.Extensions;

public static class WebApplicationExtensions
{
    public static void ConfigureApplication(this WebApplication app)
    {
        if (app == null)
        {
            throw new ArgumentNullException(nameof(app));
        }

        var supportedCultures = new[] { "en", "fa" };
        var localizationOptions = new RequestLocalizationOptions()
            .SetDefaultCulture(supportedCultures[0])
            .AddSupportedCultures(supportedCultures)
            .AddSupportedUICultures(supportedCultures);

        localizationOptions.ApplyCurrentCultureToResponseHeaders = true;
        app.UseRequestLocalization(localizationOptions);

        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            // Hide schema
            options.DefaultModelsExpandDepth(-1);
            options.EnableDeepLinking();
        });

        app.UseHttpsRedirection();

        app.AddAllModules();

        app.UseCors("CorsPolicy");

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseMiddleware<ErrorHandlerMiddleware>();
    }

    public async static void ConfigureDatabase(this WebApplication app)
    {
        if (app == null)
        {
            throw new ArgumentNullException(nameof(app));
        }

        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            if (context.Database.IsSqlServer())
            {
                context.Database.Migrate();
            }

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            await ApplicationDbContextSeed.SeedDataAsync(context);
            await ApplicationDbContextSeed.SeedAccountingDataAsync(context);
            await ApplicationDbContextSeed.SeedDefaultRolesAndUserAsync(userManager, roleManager);
        }
    }

    private static void AddAllModules(this WebApplication app)
    {
        var modules = typeof(Program).Assembly
            .GetTypes()
            .Where(t => t.IsAssignableTo(typeof(IModule)) && !t.IsAbstract && !t.IsInterface)
            .Select(Activator.CreateInstance)
            .Cast<IModule>();

        foreach (var module in modules)
        {
            module.RegisterEndpoints(app);
        }
    }
}