using System.Reflection;
using System.Text;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Minimal.Api.Behaviors;
using Minimal.Api.Common.IdentityServices;
using Minimal.DataAccess;
using Minimal.Domain.Identity;

namespace Minimal.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        services.AddLocalization(options => options.ResourcesPath = "Resources");

        services.AddSwagger();
        services.AddIdentityOptions(config);
        services.AddPersistence(config);

        services.AddHttpContextAccessor();
        services.AddCustomServices();

        services.AddAutoMapper(typeof(Program));
        services.AddValidatorsFromAssemblyContaining(typeof(Program));
        services.AddMediatR(typeof(Program));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));

        return services;
    }

    private static void AddSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Minimal API", Version = "v1" });
            options.TagActionsBy(ta => new List<string> { ta.ActionDescriptor.DisplayName! });

            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "JWT Authentication",
                Description = "Enter JWT Bearer token",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer", // must be lower case
                BearerFormat = "JWT",
                Reference = new OpenApiReference
                {
                    Id = JwtBearerDefaults.AuthenticationScheme,
                    Type = ReferenceType.SecurityScheme
                }
            };
            options.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
            options.AddSecurityRequirement(new OpenApiSecurityRequirement { { securityScheme, new string[] { } } });

            // using System.Reflection;
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });
    }

    private static void AddIdentityOptions(this IServiceCollection services, IConfiguration config)
    {
        services.AddDefaultIdentity<ApplicationUser>(option =>
        {
            option.Password.RequiredLength = config.GetValue<int>("PasswordOptions:RequiredLength");
            option.Password.RequireDigit = config.GetValue<bool>("PasswordOptions:RequireDigit");
            option.Password.RequireLowercase = config.GetValue<bool>("PasswordOptions:RequireLowercase");
            option.Password.RequireUppercase = config.GetValue<bool>("PasswordOptions:RequireUppercase");
            option.Password.RequireNonAlphanumeric = config.GetValue<bool>("PasswordOptions:RequireNonAlphanumeric");
        })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = config.GetValue<string>("Jwt:Issuer"),
                    ValidAudience = config.GetValue<string>("Jwt:Audience"),
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetValue<string>("Jwt:Key")))
                };
            });
    }

    private static void AddPersistence(this IServiceCollection services, IConfiguration config)
    {
        var connectionString = config.GetConnectionString("SqlServer");
        if (connectionString != null)
        {
            services.AddConfiguredMsSqlDbContext(connectionString);
        }
    }

    public static void AddCustomServices(this IServiceCollection services)
    {
        services.AddScoped<ISecurityService, SecurityService>();
    }
}