using MediatR;
using Minimal.Api.Features.LoanTypes.Models;
using Minimal.Api.Models;

namespace Minimal.Api.Features.LoanTypes.Queries;

public class GetAllLoanType : PagingData, IRequest<PageList<LoanTypeGetDto>>
{
    public bool? IsActive { get; set; }
}