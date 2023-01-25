using MediatR;
using Minimal.Api.Features.Accounts.Models;

namespace Minimal.Api.Features.Accounts.Commands;

public class CloseAccount : IRequest<AccountGetDto>
{
    public int Id { get; set; }
    public DateTimeOffset CloseDate { get; set; }
}