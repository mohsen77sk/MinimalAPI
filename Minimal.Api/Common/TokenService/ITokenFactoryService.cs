using Minimal.Domain.Identity;

namespace Minimal.Api.Common.TokenService;

public interface ITokenFactoryService
{
    Task<string> CreateAccessTokenAsync(ApplicationUser user, bool rememberMe);
}