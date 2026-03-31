using Minimal.Domain;

namespace Minimal.Api.Features.Loans.Models;

public class LoanAccountingResult
{
    public AccountDetail AccountDetail { get; set; } = default!;

    public List<Document> Documents { get; set; } = new();
}