using Minimal.Api.Models;
using Minimal.Domain;

namespace Minimal.Api.Features.Loans.Models;

public class LoanInstallmentsGetDto
{
    public int Id { get; init; }

    public int Number { get; set; }

    public DateTimeOffset DueDate { get; set; }

    public DateTimeOffset? PaidDate { get; set; }

    public decimal Amount { get; set; }

    public decimal PaidAmount { get; set; }

    public InstallmentStatus Status { get; set; }
}