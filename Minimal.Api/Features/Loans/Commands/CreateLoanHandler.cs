using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Minimal.Api.Common.Accounting;
using Minimal.Api.Common.Accounting.Validators;
using Minimal.Api.Exceptions;
using Minimal.Api.Features.Loans.Models;
using Minimal.Api.Features.Loans.Profiles;
using Minimal.DataAccess;
using Minimal.Domain;

namespace Minimal.Api.Features.Loans.Commands;

public class CreateLoanHandler : IRequestHandler<CreateLoan, LoanGetDto>
{
    private readonly ApplicationDbContext _context;
    private readonly LoanMapper _mapper;
    private readonly IStringLocalizer _localizer;
    private readonly DocumentValidator _documentValidator;

    public CreateLoanHandler(
        ApplicationDbContext context,
        LoanMapper mapper,
        IStringLocalizer<SharedResource> localizer,
        DocumentValidator documentValidator)
    {
        _context = context ??
            throw new ArgumentNullException(nameof(context));
        _mapper = mapper ??
            throw new ArgumentNullException(nameof(mapper));
        _localizer = localizer ??
            throw new ArgumentNullException(nameof(localizer));
        _documentValidator = documentValidator ??
            throw new ArgumentNullException(nameof(documentValidator));
    }

    public async Task<LoanGetDto> Handle(CreateLoan request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var loanToAdd = _mapper.MapToLoan(request);

        var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id.Equals(request.AccountId), cancellationToken);
        if (account is null)
        {
            throw new ValidationException(nameof(request.AccountId), _localizer.GetString("notFound").Value);
        }

        if (account.IsActive is false)
        {
            throw new ValidationException(nameof(request.AccountId), _localizer.GetString("accountIsNotActive").Value);
        }

        var loanType = await _context.LoanTypes.FirstOrDefaultAsync(lt => lt.Id.Equals(request.LoanTypeId), cancellationToken);
        if (loanType is null)
        {
            throw new ValidationException(nameof(request.LoanTypeId), _localizer.GetString("notFound").Value);
        }

        if (loanType.IsActive is false)
        {
            throw new ValidationException(nameof(request.LoanTypeId), _localizer.GetString("loanTypeIsNotActive").Value);
        }

        var wage = loanToAdd.Amount * loanToAdd.InterestRates / 100;
        var installmentAmount = loanToAdd.Amount / loanToAdd.InstallmentCount;

        if (installmentAmount - (long)installmentAmount > 0)
        {
            throw new ValidationException("InstallmentAmount", _localizer.GetString("installmentIsDecimal").Value);
        }

        loanToAdd.Account = account;
        loanToAdd.LoanType = loanType;
        loanToAdd.InstallmentAmount = installmentAmount;
        loanToAdd.StartInstallmentPayment = loanToAdd.CreateDate.AddMonths(loanToAdd.InstallmentInterval);
        loanToAdd.IsActive = true;
        _context.Loans.Add(loanToAdd);

        var accountDetailToAdd = new AccountDetail
        {
            Title = $"تسهیلات {loanToAdd.Code}",
            Loan = loanToAdd,
            AccountCategory = await _context.GetAccountCategoryByCodeAsync("3", cancellationToken),
            IsActive = true
        };
        _context.AccountDetails.Add(accountDetailToAdd);

        var documentsToAdd = new List<Document>
        {
            new Document
            {
                Date = loanToAdd.CreateDate,
                FiscalYear = await _context.GetCurrentFiscalYearAsync(cancellationToken),
                DocumentType = await _context.GetDocumentTypeByCodeAsync("20", cancellationToken),
                DocumentItems =
                [
                    new DocumentArticle
                    {
                        AccountSubsid = await _context.GetAccountSubsidByCodeAsync(loanType.Code, cancellationToken),
                        AccountDetail = accountDetailToAdd,
                        Credit = 0,
                        Debit = request.Amount,
                        Note = ""
                    },
                    new DocumentArticle
                    {
                        AccountSubsid = await _context.GetBankAccountAsync(cancellationToken),
                        Credit = request.Amount,
                        Debit = 0,
                        Note = ""
                    }
                ],
                Note = string.Empty,
                IsActive = true
            }
        };

        if (wage > 0)
        {
            documentsToAdd.Add(new Document
            {
                Date = loanToAdd.CreateDate,
                FiscalYear = await _context.GetCurrentFiscalYearAsync(cancellationToken),
                DocumentType = await _context.GetDocumentTypeByCodeAsync("21", cancellationToken),
                DocumentItems =
                [
                    new DocumentArticle
                    {
                        AccountSubsid = await _context.GetAccountSubsidByCodeAsync(loanType.Code, cancellationToken),
                        AccountDetail = accountDetailToAdd,
                        Credit = 0,
                        Debit = wage,
                        Note = ""
                    },
                    new DocumentArticle
                    {
                        AccountSubsid = await _context.GetAccountSubsidByCodeAsync("3101", cancellationToken),
                        Credit = wage,
                        Debit = 0,
                        Note = ""
                    },
                ],
                Note = string.Empty,
                IsActive = true
            });
        }

        foreach (var doc in documentsToAdd)
        {
            var validation = _documentValidator.ValidateDocument(doc);
            if (!validation.IsValid)
            {
                throw new ErrorException(validation.ErrorMessage);
            }
        }

        _context.Documents.AddRange(documentsToAdd);

        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.MapToLoanGetDto(loanToAdd);
    }
}