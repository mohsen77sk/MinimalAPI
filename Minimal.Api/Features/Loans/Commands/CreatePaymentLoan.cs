using MediatR;
using Minimal.Api.Features.Loans.Models;

namespace Minimal.Api.Features.Loans.Commands;

public class CreatePaymentLoan : IRequest<LoanPaymentsGetDto>
{
    public int LoanId { get; set; }
    public DateTimeOffset PaymentDate { get; set; }
    public decimal Amount { get; set; }
    public string Note { get; set; } = string.Empty;
}