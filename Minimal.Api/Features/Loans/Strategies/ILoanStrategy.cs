using Minimal.Api.Features.Loans.Commands;
using Minimal.Api.Features.Loans.Models;
using Minimal.DataAccess;
using Minimal.Domain;

namespace Minimal.Api.Features.Loans.Strategies;

public interface ILoanStrategy
{
    Loan BuildLoan(CreateLoan request, Account account, LoanType loanType);

    List<LoanInstallment> GenerateInstallments(Loan loan);

    LoanDisbursementResult CalculateDisbursement(Loan loan);

    LoanAccountingResult CreateAccounting(Loan loan, LoanDisbursementResult disbursement, LoanAccountingContext context);
}