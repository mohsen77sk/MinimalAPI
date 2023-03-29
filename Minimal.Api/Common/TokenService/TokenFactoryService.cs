using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Minimal.Api.Common.DeviceDetectionService;
using Minimal.Api.Common.IdentityServices;
using Minimal.Domain.Identity;

namespace Minimal.Api.Common.TokenService;

public class TokenFactoryService : ITokenFactoryService
{
    private readonly IConfiguration _config;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IDeviceDetectionService _deviceDetectionService;
    private readonly ISecurityService _securityService;

    public TokenFactoryService(
        IConfiguration config,
        UserManager<ApplicationUser> userManager,
        ISecurityService securityService,
        IDeviceDetectionService deviceDetectionService)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _securityService = securityService ?? throw new ArgumentNullException(nameof(securityService));
        _deviceDetectionService = deviceDetectionService ?? throw new ArgumentNullException(nameof(deviceDetectionService));
    }

    public async Task<string> CreateAccessTokenAsync(ApplicationUser user, bool rememberMe)
    {
        if (user is null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        var claims = new List<Claim> {
            // Unique Id for Jwt
            new Claim(JwtRegisteredClaimNames.Jti, _securityService.CreateCryptographicallySecureGuid().ToString(), ClaimValueTypes.String),
            // Device details
            new Claim(ClaimTypes.System, _deviceDetectionService.GetCurrentRequestDeviceDetailsHash(), ClaimValueTypes.String),
            // SecurityStamp
            new Claim(ClaimTypes.Hash, user.SecurityStamp ?? "", ClaimValueTypes.String),
            // Username
            new Claim(ClaimTypes.NameIdentifier, user.UserName ?? "", ClaimValueTypes.String),
        };

        foreach (var role in await _userManager.GetRolesAsync(user))
        {
            claims.Add(new Claim(ClaimTypes.Role, role, ClaimValueTypes.String));
        }

        var token = new JwtSecurityToken
        (
            issuer: _config.GetValue<string>("Jwt:Issuer") ?? "",
            audience: _config.GetValue<string>("Jwt:Audience") ?? "",
            claims: claims,
            expires: rememberMe ? DateTime.UtcNow.AddMonths(1) : DateTime.UtcNow.AddHours(4),
            notBefore: DateTime.UtcNow,
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetValue<string>("Jwt:Key") ?? "")),
                SecurityAlgorithms.HmacSha256
            )
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}