using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using Minimal.DataAccess;
using Minimal.Domain.Identity;

namespace Minimal.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, WebApplicationBuilder builder)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        builder.Services.AddSwagger();
        builder.Services.AddIdentityOptions(builder.Configuration);
        builder.Services.AddPersistence(builder.Configuration);

        builder.Services.AddAutoMapper(typeof(Program));
        builder.Services.AddMediatR(typeof(Program));

        return services;
    }

    private static void AddSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Minimal API", Version = "v1" });
            options.TagActionsBy(ta => new List<string> { ta.ActionDescriptor.DisplayName! });
        });

    }

    private static void AddIdentityOptions(this IServiceCollection services, IConfiguration config)
    {
        services.AddDefaultIdentity<ApplicationUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();
    }

    private static void AddPersistence(this IServiceCollection services, IConfiguration config)
    {
        var connectionString = config.GetConnectionString("SqlServer");
        services.AddConfiguredMsSqlDbContext(connectionString);
    }
}