using MediatR;
using Minimal.Api.Features.Accounts.Models;

namespace Minimal.Api.Features.Accounts.Commands;

public class UpdateAccount : IRequest<AccountGetDto>
{
    public int Id { get; set; }
    public required List<int> PersonId { get; set; }
    public string Note { get; set; } = string.Empty;
}