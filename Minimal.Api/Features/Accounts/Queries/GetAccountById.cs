using MediatR;
using Minimal.Api.Features.Accounts.Models;

namespace Minimal.Api.Features.Accounts.Queries;

public class GetAccountById : IRequest<AccountGetDto>
{
    public int AccountId { get; set; }
}