using MediatR;
using Minimal.Api.Features.Loans.Models;

namespace Minimal.Api.Features.Loans.Commands;

public class CreateLoan : IRequest<LoanGetDto>
{
    public int AccountId { get; set; }
    public int LoanTypeId { get; set; }
    public DateTimeOffset CreateDate { get; set; }
    public decimal Amount { get; set; }
    public int InstallmentCount { get; set; }
    public int InstallmentInterval { get; set; }
    public int InterestRates { get; set; }
    public string Note { get; set; } = string.Empty;
}