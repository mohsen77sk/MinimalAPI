using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Minimal.Api.Common.Accounting;
using Minimal.Api.Common.Accounting.Validators;
using Minimal.Api.Exceptions;
using Minimal.Api.Features.Loans.Models;
using Minimal.Api.Features.Loans.Profiles;
using Minimal.Api.Features.Loans.Strategies;
using Minimal.DataAccess;
using Minimal.Domain;

namespace Minimal.Api.Features.Loans.Commands;

public class CreateLoanHandler : IRequestHandler<CreateLoan, LoanGetDto>
{
    private readonly ApplicationDbContext _context;
    private readonly LoanMapper _mapper;
    private readonly IStringLocalizer _localizer;
    private readonly DocumentValidator _documentValidator;
    private readonly ILoanStrategyFactory _strategyFactory;

    public CreateLoanHandler(
        ApplicationDbContext context,
        LoanMapper mapper,
        IStringLocalizer<SharedResource> localizer,
        DocumentValidator documentValidator,
        ILoanStrategyFactory strategyFactory)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
        _documentValidator = documentValidator ?? throw new ArgumentNullException(nameof(documentValidator));
        _strategyFactory = strategyFactory ?? throw new ArgumentNullException(nameof(strategyFactory));
    }

    public async Task<LoanGetDto> Handle(CreateLoan request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

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

        var strategy = _strategyFactory.GetStrategy(loanType.Strategy);

        var loan = strategy.BuildLoan(request, account, loanType);
        _context.Loans.Add(loan);

        var installments = strategy.GenerateInstallments(loan);
        _context.LoanInstallments.AddRange(installments);

        var disbursement = strategy.CalculateDisbursement(loan);

        var accountingContext = new LoanAccountingContext
        {
            FiscalYear = await _context.GetCurrentFiscalYearAsync(cancellationToken),
            LoanAccountCategory = await _context.GetAccountCategoryByCodeAsync("3", cancellationToken),
            BankAccountSubsid = await _context.GetBankAccountAsync(cancellationToken),
            LoanAccountSubsid = await _context.GetAccountSubsidByCodeAsync(loanType.Code, cancellationToken),
            FeeAccountSubsid = await _context.GetAccountSubsidByCodeAsync("3102", cancellationToken),
            LoanDocumentType = await _context.GetDocumentTypeByCodeAsync("20", cancellationToken),
            InterestDocumentType = await _context.GetDocumentTypeByCodeAsync("21", cancellationToken)
        };

        var accounting = strategy.CreateAccounting(loan, disbursement, accountingContext);

        foreach (var doc in accounting.Documents)
        {
            var validation = _documentValidator.ValidateDocument(doc);
            if (!validation.IsValid)
            {
                throw new ErrorException(validation.ErrorMessage);
            }
        }

        _context.AccountDetails.Add(accounting.AccountDetail);
        _context.Documents.AddRange(accounting.Documents);

        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.MapToLoanGetDto(loan);
    }
}