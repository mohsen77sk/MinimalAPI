using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Minimal.Api.Exceptions;
using Minimal.Api.Features.Accounts.Models;
using Minimal.DataAccess;
using Minimal.Domain;

namespace Minimal.Api.Features.Accounts.Commands;

public class CloseAccountHandler : IRequestHandler<CloseAccount, AccountGetDto>
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer _localizer;

    public CloseAccountHandler(ApplicationDbContext context, IMapper mapper, IStringLocalizer<SharedResource> localizer)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
    }

    public async Task<AccountGetDto> Handle(CloseAccount request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var account = await _context.Accounts
            .Include(a => a.People)
            .Include(a => a.AccountType)
            .Include(a => a.AccountDetail)
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);
        if (account is null)
        {
            throw new NotFoundException(_localizer.GetString("notFoundAccount").Value);
        }

        if (account.IsActive is false)
        {
            throw new ValidationException(nameof(request.Id), _localizer.GetString("accountIsNotActive").Value);
        }

        if (account.CreateDate >= request.CloseDate)
        {
            throw new ValidationException(nameof(request.CloseDate), _localizer.GetString("closingAccountDateIsBeforeOpeningDate").Value);
        }

        if (await _context.DocumentArticles.AnyAsync(dr =>
            dr.Document.IsActive == true &&
            dr.AccountDetailId == account.AccountDetail.Id &&
            dr.Document.Date >= request.CloseDate, cancellationToken))
        {
            throw new ValidationException(nameof(request.CloseDate), _localizer.GetString("closingAccountDateIsBeforeTransaction").Value);
        }

        var accountBalance = await _context.DocumentArticles
            .Where(da => da.AccountDetailId == account.AccountDetail.Id && da.Document.IsActive == true)
            .SumAsync(da => da.Credit - da.Debit, cancellationToken);

        var documentToAdd = new Document
        {
            Date = request.CloseDate,
            Note = "سند اختتامیه ی حساب" + " " + account.Code,
            FiscalYear = await _context.FiscalYears.SingleAsync(f => f.Id == 1, cancellationToken),
            DocumentType = await _context.DocumentTypes.SingleAsync(dt => dt.Code == "11", cancellationToken),
            DocumentItems = new List<DocumentArticle>()
            {
                new DocumentArticle
                {
                    AccountSubsid = await _context.AccountSubsids.SingleAsync(x => x.Code == account.AccountType.Code, cancellationToken),
                    AccountDetail = account.AccountDetail,
                    Credit = 0,
                    Debit = accountBalance,
                    Note = ""
                },
                new DocumentArticle
                {
                    AccountSubsid = await _context.AccountSubsids.SingleAsync(x => x.Code == "1101", cancellationToken),
                    AccountDetail = await _context.AccountDetails.SingleAsync(x => x.Code == "11010001", cancellationToken),
                    Credit = accountBalance,
                    Debit = 0,
                    Note = ""
                }
            },
            IsActive = true,
        };
        _context.Documents.Add(documentToAdd);

        account.CloseDate = request.CloseDate;
        account.IsActive = false;
        account.AccountDetail.IsActive = false;

        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<AccountGetDto>(account);
    }
}