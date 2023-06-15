namespace Minimal.Api.Features.Auth.Models;

public class AuthenticationDto
{
    public bool? RequiresTwoFactor { get; set; }

    public string? AccessToken { get; set; }
}
