using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Minimal.Api.Common.TokenService;
using Minimal.Api.Contracts;
using Minimal.Api.Extensions;
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
        ITokenFactoryService tokenFactoryService,
        IStringLocalizer<SharedResource> localizer,
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

            return Results.Ok(new AuthenticationDto { AccessToken = await tokenFactoryService.CreateAccessTokenAsync(user, model.RememberMe) });
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
            var user = await userManager.FindByNameAsync(userIdentity.GetUserName() ?? "");
            if (user != null) await userManager.UpdateSecurityStampAsync(user);
        }
        await signInManager.SignOutAsync();
        return Results.NoContent();
    }
}