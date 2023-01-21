using MediatR;
using Minimal.Api.Features.Accounts.Models;

namespace Minimal.Api.Features.Accounts.Queries;

public class GetAccountBalance : IRequest<AccountBalanceGetDto>
{
    public int AccountId { get; set; }
}