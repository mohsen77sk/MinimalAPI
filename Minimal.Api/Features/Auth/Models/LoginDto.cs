namespace Minimal.Api.Features.Auth.Models;

public class LoginDto
{
    public required string Username { get; set; }

    public required string Password { get; set; }

    public bool RememberMe { get; set; }
}
