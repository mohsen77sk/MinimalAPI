namespace Minimal.Api.Features.AccountTypes.Models;

public class AccountTypeGetDto
{
    public int Id { get; init; }

    public string Code { get; init; } = default!;

    public string Name { get; init; } = default!;

    public bool IsActive { get; init; }
}
