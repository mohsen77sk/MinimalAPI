using MediatR;
using Minimal.Api.Features.FundAccount.Models;

namespace Minimal.Api.Features.FundAccount.Queries;

public class GetFundAccountBalance : IRequest<FundAccountBalanceGetDto>
{

}