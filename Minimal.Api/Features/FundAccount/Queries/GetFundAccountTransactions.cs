using MediatR;
using Minimal.Api.Features.FundAccount.Models;
using Minimal.Api.Models;

namespace Minimal.Api.Features.FundAccount.Queries;

public class GetFundAccountTransactions : PagingData, IRequest<PageList<FundAccountTransactionGetDto>>
{

}