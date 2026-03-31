using MediatR;
using Minimal.Api.Features.Loans.Models;

namespace Minimal.Api.Features.Loans.Queries;

public class GetLoanPayments : IRequest<List<LoanPaymentsGetDto>>
{
    public int LoanId { get; set; }
}