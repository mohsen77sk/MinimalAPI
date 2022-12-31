using System.Security.Claims;
using System.Security.Principal;

namespace Minimal.Api.Extensions;

public static class IdentityExtensions
{
    public static string? GetUserId(this IIdentity identity)
    {
        var claimsIdentity = identity as ClaimsIdentity;
        return claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }

    public static string[]? GetRoles(this IIdentity identity)
    {
        var claimsIdentity = identity as ClaimsIdentity;
        return claimsIdentity?.FindAll(ClaimTypes.Role).Select(x => x.Value).ToArray<string>();
    }
}