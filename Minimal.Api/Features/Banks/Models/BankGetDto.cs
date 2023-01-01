namespace Minimal.Api.Features.Banks.Models;

public class BankGetDto
{
    public int Id { get; init; }

    public string Code { get; init; } = default!;

    public string Name { get; init; } = default!;

    public bool IsActive { get; init; }
}
