namespace Minimal.Api.Features.People.Models;

public class PeopleGetDto
{
    public int Id { get; init; }

    public string Code { get; init; } = default!;

    public string FullName { get; init; } = default!;

    public bool IsActive { get; init; }
}
