using MediatR;
using Minimal.Api.Features.Loans.Models;

namespace Minimal.Api.Features.Loans.Queries;

public class GetLoanInstallments : IRequest<List<LoanInstallmentsGetDto>>
{
    public int LoanId { get; set; }
}