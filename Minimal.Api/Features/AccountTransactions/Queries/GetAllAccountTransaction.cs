using MediatR;
using Minimal.Api.Features.AccountTransactions.Models;
using Minimal.Api.Models;

namespace Minimal.Api.Features.AccountTransactions.Queries;

public class GetAllAccountTransaction : PagingData, IRequest<PageList<AccountTransactionGetDto>>
{
    public int AccountId { get; set; }
}