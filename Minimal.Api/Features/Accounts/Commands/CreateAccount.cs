using MediatR;
using Minimal.Api.Features.Accounts.Models;

namespace Minimal.Api.Features.Accounts.Commands;

public class CreateAccount : IRequest<AccountGetDto>
{
    public int AccountTypeId { get; set; }
    public decimal InitCredit { get; set; }
    public DateTimeOffset CreateDate { get; set; }
    public required List<int> PersonId { get; set; }
    public string Note { get; set; } = string.Empty;
}