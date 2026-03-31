using Minimal.Domain;

namespace Minimal.Api.Features.Loans.Models;

public class LoanAccountingContext
{
    public FiscalYear FiscalYear { get; set; } = default!;

    public AccountCategory LoanAccountCategory { get; set; } = default!;

    public AccountSubsid LoanAccountSubsid { get; set; } = default!;

    public AccountSubsid BankAccountSubsid { get; set; } = default!;

    public AccountSubsid FeeAccountSubsid { get; set; } = default!;

    public DocumentType LoanDocumentType { get; set; } = default!;

    public DocumentType InterestDocumentType { get; set; } = default!;
}