using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Minimal.Api.Exceptions;
using Minimal.Api.Features.Loans.Models;
using Minimal.DataAccess;
using Minimal.Domain;

namespace Minimal.Api.Features.Loans.Commands;

public class CreateLoanHandler : IRequestHandler<CreateLoan, LoanGetDto>
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer _localizer;

    public CreateLoanHandler(ApplicationDbContext context, IMapper mapper, IStringLocalizer<SharedResource> localizer)
    {
        _context = context ??
            throw new ArgumentNullException(nameof(context));
        _mapper = mapper ??
            throw new ArgumentNullException(nameof(mapper));
        _localizer = localizer ??
            throw new ArgumentNullException(nameof(localizer));
    }

    public async Task<LoanGetDto> Handle(CreateLoan request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var loanToAdd = _mapper.Map<Loan>(request);

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
            AccountCategory = await _context.AccountCategories.SingleAsync(ac => ac.Code == "3", cancellationToken),
            IsActive = true
        };
        _context.AccountDetails.Add(accountDetailToAdd);

        var documentsToAdd = new List<Document>
        {
            new Document
            {
                Date = loanToAdd.CreateDate,
                FiscalYear = await _context.FiscalYears.SingleAsync(f => f.Id == 1, cancellationToken),
                DocumentType = await _context.DocumentTypes.SingleAsync(dt => dt.Code == "20", cancellationToken),
                DocumentItems =
                [
                    new DocumentArticle
                    {
                        AccountSubsid = await _context.AccountSubsids.SingleAsync(x => x.Code == loanType.Code, cancellationToken),
                        AccountDetail = accountDetailToAdd,
                        Credit = 0,
                        Debit = request.Amount,
                        Note = ""
                    },
                    new DocumentArticle
                    {
                        AccountSubsid = await _context.AccountSubsids.SingleAsync(x => x.Code == "1101", cancellationToken),
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
                FiscalYear = await _context.FiscalYears.SingleAsync(f => f.Id == 1, cancellationToken),
                DocumentType = await _context.DocumentTypes.SingleAsync(dt => dt.Code == "21", cancellationToken),
                DocumentItems =
                [
                    new DocumentArticle
                    {
                        AccountSubsid = await _context.AccountSubsids.SingleAsync(x => x.Code == loanType.Code, cancellationToken),
                        AccountDetail = accountDetailToAdd,
                        Credit = 0,
                        Debit = wage,
                        Note = ""
                    },
                    new DocumentArticle
                    {
                        AccountSubsid = await _context.AccountSubsids.SingleAsync(x => x.Code == "3101", cancellationToken),
                        Credit = wage,
                        Debit = 0,
                        Note = ""
                    },
                ],
                Note = string.Empty,
                IsActive = true
            });
        }
        _context.Documents.AddRange(documentsToAdd);

        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<LoanGetDto>(loanToAdd);
    }
}