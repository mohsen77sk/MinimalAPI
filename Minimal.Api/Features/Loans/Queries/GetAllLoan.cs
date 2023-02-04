using MediatR;
using Minimal.Api.Features.Loans.Models;
using Minimal.Api.Models;

namespace Minimal.Api.Features.Loans.Queries;

public class GetAllLoan : PagingData, IRequest<PageList<LoanGetDto>>
{
}