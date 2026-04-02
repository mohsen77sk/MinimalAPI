using DNTPersianUtils.Core;
using Minimal.Api.Common.Accounting;
using Minimal.Api.Features.Loans.Commands;
using Minimal.Api.Features.Loans.Models;
using Minimal.Domain;

namespace Minimal.Api.Features.Loans.Strategies;

public class GharzolHasanehLoanStrategy : ILoanStrategy
{
    public Loan BuildLoan(CreateLoan request, Account account, LoanType loanType)
    {
        var installmentAmount = Math.Round(request.Amount / request.InstallmentCount, 0);

        var loan = new Loan
        {
            Account = account,
            LoanType = loanType,
            CreateDate = request.CreateDate,
            Amount = request.Amount,
            InstallmentCount = request.InstallmentCount,
            InstallmentInterval = request.InstallmentInterval,
            InterestRates = request.InterestRates,
            InstallmentAmount = installmentAmount,
            StartInstallmentPayment = new DateTimeOffset(request.CreateDate.AddMonths(request.InstallmentInterval).Date, TimeSpan.Zero),
            Note = request.Note,
            IsActive = true,
        };

        return loan;
    }

    public List<LoanInstallment> GenerateInstallments(Loan loan)
    {
        var installments = new List<LoanInstallment>();

        var totalInterest = loan.Amount * loan.InterestRates / 100;

        var totalPayable = loan.Amount + totalInterest;

        var baseAmount = totalPayable / loan.InstallmentCount;

        decimal assigned = 0;

        for (int i = 1; i <= loan.InstallmentCount; i++)
        {
            decimal amount;

            if (i == loan.InstallmentCount)
            {
                amount = totalPayable - assigned;
            }
            else
            {
                amount = baseAmount;
                assigned += amount;
            }

            var interestPart = totalInterest / loan.InstallmentCount;
            var principalPart = amount - interestPart;

            installments.Add(new LoanInstallment
            {
                Loan = loan,
                Number = i,
                PrincipalAmount = principalPart,
                InterestAmount = interestPart,
                PaidPrincipal = 0,
                PaidInterest = 0,
                DueDate = loan.StartInstallmentPayment.AddMonths((i - 1) * loan.InstallmentInterval),
                Status = InstallmentStatus.Pending
            });
        }

        return installments;
    }

    public LoanDisbursementResult CalculateDisbursement(Loan loan)
    {
        var fee = (long)Math.Round(loan.Amount * 0.01m, 0);

        return new LoanDisbursementResult
        {
            NetAmount = loan.Amount - fee,
            FeeAmount = fee
        };
    }

    public LoanAccountingResult CreateAccounting(Loan loan, LoanDisbursementResult disbursement, LoanAccountingContext context)
    {
        var accountDetail = new AccountDetail
        {
            Title = $"تسهیلات {loan.Code}",
            Loan = loan,
            AccountCategory = context.LoanAccountCategory,
            IsActive = true
        };

        var documents = new List<Document>();

        var loanDoc = new AccountingDocumentBuilder(context.FiscalYear, context.LoanDocumentType, loan.CreateDate)
            .Debit(loan.Amount, context.LoanAccountSubsid, accountDetail)
            .Credit(disbursement.NetAmount, context.BankAccountSubsid);

        if (disbursement.FeeAmount > 0)
        {
            loanDoc.Credit(disbursement.FeeAmount, context.FeeAccountSubsid);
        }

        documents.Add(loanDoc.Build());

        return new LoanAccountingResult
        {
            AccountDetail = accountDetail,
            Documents = documents
        };
    }

}