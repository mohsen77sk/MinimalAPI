using Minimal.Api.Models;
using Minimal.Domain;

namespace Minimal.Api.Features.Loans.Models;

public class LoanPaymentsGetDto
{
    public int Id { get; init; }

    public decimal Amount { get; set; }

    public DateTimeOffset PaymentDate { get; set; }

    public int DocumentId { get; set; }

    public string DocumentCode { get; set; } = default!;

    public string Note { get; set; } = default!;
}