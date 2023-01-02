using MediatR;
using Minimal.Api.Features.Accounts.Models;

namespace Minimal.Api.Features.Accounts.Commands;

public class UpdateAccount : IRequest<AccountGetDto>
{
    public int Id { get; set; }
    public int AccountTypeId { get; set; }
    public decimal InitCredit { get; set; }
    public DateTimeOffset CreateDate { get; set; }
    public required List<int> PersonId { get; set; }
    public string Note { get; set; } = string.Empty;
}