using MediatR;
using Minimal.Api.Features.Loans.Models;

namespace Minimal.Api.Features.Loans.Commands;

public class CreateReversePaymentLoan : IRequest<LoanPaymentsGetDto>
{
    public int Id { get; set; }
    public int LoanId { get; set; }
    public string Note { get; set; } = string.Empty;
}