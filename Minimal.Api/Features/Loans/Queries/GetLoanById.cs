using MediatR;
using Minimal.Api.Features.Loans.Models;

namespace Minimal.Api.Features.Loans.Queries;

public class GetLoanById : IRequest<LoanGetDto>
{
    public int LoanId { get; set; }
}