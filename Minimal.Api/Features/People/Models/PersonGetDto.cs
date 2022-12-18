namespace Minimal.Api.Features.People.Models;

public class PersonGetDto
{
    public int Id { get; init; }

    public string Code { get; init; } = default!;

    public string FirstName { get; init; } = default!;

    public string LastName { get; init; } = default!;

    public string NationalCode { get; set; } = default!;

    public DateTime? DateOfBirth { get; init; }

    public byte Gender { get; init; }

    public string? Note { get; init; }

    public bool IsActive { get; init; }
}
