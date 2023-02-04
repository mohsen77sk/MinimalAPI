using MediatR;
using Minimal.Api.Features.LoanTransactions.Models;
using Minimal.Api.Models;

namespace Minimal.Api.Features.LoanTransactions.Queries;

public class GetAllLoanTransaction : PagingData, IRequest<PageList<LoanTransactionGetDto>>
{
    public int LoanId { get; set; }
}