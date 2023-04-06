using System.Reflection;
using System.Text;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Minimal.Api.Behaviors;
using Minimal.Api.Common.DeviceDetectionService;
using Minimal.Api.Common.IdentityServices;
using Minimal.Api.Common.TokenService;
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
        services.AddCustomServices();
        services.AddPersistence(config);
        services.AddIdentityOptions(config);
        services.AddCustomAuthentication(config);
        services.AddCustomCors();

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

            options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
            {
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Name = "Authorization",
                Description = "Bearer Authentication with JWT Token",
                Type = SecuritySchemeType.Http
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Id = JwtBearerDefaults.AuthenticationScheme,
                            Type = ReferenceType.SecurityScheme
                        }
                    },
                    new List<string>()
                }
            });

            // using System.Reflection;
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });
    }

    private static void AddPersistence(this IServiceCollection services, IConfiguration config)
    {
        var connectionString = config.GetConnectionString("SqlServer");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("connection string is null");
        }
        services.AddConfiguredMsSqlDbContext(connectionString);
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
    }

    private static void AddCustomAuthentication(this IServiceCollection services, IConfiguration config)
    {
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = config.GetValue<string>("Jwt:Issuer") ?? "",
                    ValidAudience = config.GetValue<string>("Jwt:Audience") ?? "",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetValue<string>("Jwt:Key") ?? ""))
                };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var tokenValidatorService = context.HttpContext.RequestServices.GetRequiredService<ITokenValidatorService>();
                        return tokenValidatorService.ValidateAsync(context);
                    },
                };

                options.SaveToken = true;
            });
    }

    private static void AddCustomCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(name: "CorsPolicy", builder =>
            {
                builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
            });
        });
    }

    private static void AddCustomServices(this IServiceCollection services)
    {
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddScoped<IDeviceDetectionService, DeviceDetectionService>();
        services.AddSingleton<ISecurityService, SecurityService>();
        services.AddScoped<ITokenFactoryService, TokenFactoryService>();
        services.AddScoped<ITokenValidatorService, TokenValidatorService>();
    }
}