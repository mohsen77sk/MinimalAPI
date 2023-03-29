using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Minimal.Api.Common.DeviceDetectionService;
using Minimal.Domain.Identity;

namespace Minimal.Api.Common.TokenService;

public class TokenValidatorService : ITokenValidatorService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IDeviceDetectionService _deviceDetectionService;

    public TokenValidatorService(UserManager<ApplicationUser> userManager, IDeviceDetectionService deviceDetectionService)
    {
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _deviceDetectionService = deviceDetectionService ?? throw new ArgumentNullException(nameof(deviceDetectionService));
    }

    public async Task ValidateAsync(TokenValidatedContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        var claimsIdentity = context.Principal?.Identity as ClaimsIdentity;
        if (claimsIdentity?.Claims == null || !claimsIdentity.Claims.Any())
        {
            context.Fail("This is not our issued token. It has no claims.");
            return;
        }

        var systemClaim = claimsIdentity.FindFirst(ClaimTypes.System)?.Value;
        if (!string.Equals(_deviceDetectionService.GetCurrentRequestDeviceDetailsHash(), systemClaim, StringComparison.Ordinal))
        {
            context.Fail("Detected usage of an old token from a new device! Please login again!");
            return;
        }

        var securityStampClaim = claimsIdentity.FindFirst(ClaimTypes.Hash)?.Value;
        if (securityStampClaim == null)
        {
            context.Fail("This is not our issued token. It has no hash.");
            return;
        }

        var userName = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userName))
        {
            context.Fail("This is not our issued token. It has no username.");
            return;
        }

        var user = await _userManager.FindByNameAsync(userName);
        if (user == null || !string.Equals(user.SecurityStamp, securityStampClaim, StringComparison.Ordinal))
        {
            // user has changed his/her password/roles/stat or logout.
            context.Fail("This token is expired. Please login again.");
            return;
        }
    }
}