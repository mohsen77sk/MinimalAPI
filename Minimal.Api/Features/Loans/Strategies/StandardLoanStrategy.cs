using Microsoft.EntityFrameworkCore;
using Minimal.Api.Features.Loans.Commands;
using Minimal.Api.Features.Loans.Models;
using Minimal.DataAccess;
using Minimal.Domain;

namespace Minimal.Api.Features.Loans.Strategies;

public class StandardLoanStrategy : ILoanStrategy
{
    public Loan BuildLoan(CreateLoan request, Account account, LoanType loanType)
    {
        throw new NotImplementedException();
    }

    public List<LoanInstallment> GenerateInstallments(Loan loan)
    {
        throw new NotImplementedException();
    }

    public LoanDisbursementResult CalculateDisbursement(Loan loan)
    {
        throw new NotImplementedException();
    }

    public LoanAccountingResult CreateAccounting(Loan loan, LoanDisbursementResult disbursement, LoanAccountingContext context)
    {
        throw new NotImplementedException();
    }
}