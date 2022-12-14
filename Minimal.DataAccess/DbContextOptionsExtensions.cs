using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Minimal.DataAccess.Common;

namespace Minimal.DataAccess;

public static class DbContextOptionsExtensions
{
    public static IServiceCollection AddConfiguredMsSqlDbContext(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<ApplicationDbContext>(opt => opt
            .UseSqlServer(connectionString)
            .AddInterceptors(new PersianYeKeCommandInterceptor())
        );

        return services;
    }
}