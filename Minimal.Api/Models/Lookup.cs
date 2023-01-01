namespace Minimal.Api.Models;

public class LookupDto
{
    public int Id { get; init; }

    public string Code { get; init; } = default!;

    public string Name { get; init; } = default!;
}
