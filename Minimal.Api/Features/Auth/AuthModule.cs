using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using Minimal.Api.Common.IdentityServices;
using Minimal.Api.Contracts;
using Minimal.Api.Features.Auth.Models;
using Minimal.Api.Models;
using Minimal.Domain.Identity;

namespace Minimal.Api.Features.BankAccounts;

public class AuthModule : IModule
{
    public IEndpointRouteBuilder RegisterEndpoints(IEndpointRouteBuilder endpoints)
    {
        var authPaths = endpoints.MapGroup("/auth").WithDisplayName("Auth");

        authPaths.MapPost("/login", LoginAsync)
            .Produces<AuthenticationDto>()
            .Produces<ValidationError>(400)
            .Produces(500)
            .AllowAnonymous();

        authPaths.MapPost("/logout", LogoutAsync)
            .Produces(204)
            .Produces(500)
            .RequireAuthorization();

        return authPaths;
    }

    private async Task<IResult> LoginAsync(
        LoginDto model,
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        ISecurityService securityService,
        IStringLocalizer<SharedResource> localizer,
        IConfiguration config,
        CancellationToken ct
    )
    {
        var result = await signInManager.PasswordSignInAsync(
            model.Username,
            model.Password,
            model.RememberMe,
            lockoutOnFailure: true);

        if (result.Succeeded)
        {
            var user = await userManager.FindByNameAsync(model.Username);
            if (user is null) return Results.Problem();

            var claims = new List<Claim> {
                // Unique Id for Jwt
                new Claim(JwtRegisteredClaimNames.Jti, securityService.CreateCryptographicallySecureGuid().ToString(), ClaimValueTypes.String),
                new Claim(ClaimTypes.NameIdentifier, user.UserName ?? "", ClaimValueTypes.String)
            };

            foreach (var role in await userManager.GetRolesAsync(user))
            {
                claims.Add(new Claim(ClaimTypes.Role, role, ClaimValueTypes.String));
            }

            var token = new JwtSecurityToken
            (
                issuer: config.GetValue<string>("Jwt:Issuer") ?? "",
                audience: config.GetValue<string>("Jwt:Audience") ?? "",
                claims: claims,
                expires: model.RememberMe ? DateTime.UtcNow.AddMonths(1) : DateTime.UtcNow.AddHours(4),
                notBefore: DateTime.UtcNow,
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetValue<string>("Jwt:Key") ?? "")),
                    SecurityAlgorithms.HmacSha256
                )
            );

            return Results.Ok(new AuthenticationDto { AccessToken = new JwtSecurityTokenHandler().WriteToken(token) });
        }

        if (result.RequiresTwoFactor)
        {
            return Results.Ok(new AuthenticationDto { RequiresTwoFactor = true });
        }

        if (result.IsLockedOut)
        {
            return Results.BadRequest(new ValidationError { Message = localizer.GetString("isLockedOut").Value });
        }

        if (result.IsNotAllowed)
        {
            return Results.BadRequest(new ValidationError { Message = localizer.GetString("isNotAllowed").Value });
        }

        return Results.BadRequest(new ValidationError { Message = localizer.GetString("invalidUserNameOrPassword").Value });
    }

    private async Task<IResult> LogoutAsync(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken ct
    )
    {
        var userIdentity = httpContextAccessor.HttpContext?.User.Identity;
        if (userIdentity != null && userIdentity.IsAuthenticated)
        {
            var user = await userManager.FindByNameAsync(userIdentity.Name ?? "");
            if (user != null) await userManager.UpdateSecurityStampAsync(user);
        }
        await signInManager.SignOutAsync();
        return Results.NoContent();
    }
}