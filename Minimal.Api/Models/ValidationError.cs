namespace Minimal.Api.Models;

public class ValidationError
{
    public string Message { get; set; } = default!;
    public IDictionary<string, string[]> Errors { get; set; } = default!;
}
