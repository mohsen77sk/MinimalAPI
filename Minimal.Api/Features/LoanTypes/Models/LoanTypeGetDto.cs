namespace Minimal.Api.Features.LoanTypes.Models;

public class LoanTypeGetDto
{
    public int Id { get; init; }

    public string Code { get; init; } = default!;

    public string Name { get; init; } = default!;

    public bool IsActive { get; init; }
}
