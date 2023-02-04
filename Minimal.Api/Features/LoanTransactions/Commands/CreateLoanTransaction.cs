using MediatR;
using Minimal.Api.Features.LoanTransactions.Models;

namespace Minimal.Api.Features.LoanTransactions.Commands;

public class CreateLoanTransaction : IRequest<LoanTransactionGetDto>
{
    public int LoanId { get; set; }
    public decimal Amount { get; set; }
    public DateTimeOffset Date { get; set; }
    public string Note { get; set; } = string.Empty;
}