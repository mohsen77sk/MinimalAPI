using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Minimal.Api.Common.TokenService;

public interface ITokenValidatorService
{
    Task ValidateAsync(TokenValidatedContext context);
}