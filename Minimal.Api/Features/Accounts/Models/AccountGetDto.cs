using Minimal.Api.Models;

namespace Minimal.Api.Features.Accounts.Models;

public class AccountGetDto
{
    public int Id { get; init; }

    public string Code { get; set; } = default!;

    public int AccountTypeId { get; set; }

    public string AccountTypeName { get; set; } = default!;

    public DateTimeOffset CreateDate { get; set; }

    public DateTimeOffset? CloseDate { get; set; }

    public IList<LookupDto> Persons { get; set; } = default!;

    public string Note { get; set; } = default!;

    public bool IsActive { get; set; }
}