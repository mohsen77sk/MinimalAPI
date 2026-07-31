using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Minimal.Api.Exceptions;
using Minimal.Api.Extensions;
using Minimal.Api.Features.AccountTransactions.Models;
using Minimal.Api.Models;
using Minimal.DataAccess;

namespace Minimal.Api.Features.AccountTransactions.Queries;

public class GetAllAccountTransactionHandler : IRequestHandler<GetAllAccountTransaction, PageList<AccountTransactionGetDto>>
{
    private readonly ApplicationDbContext _context;
    private readonly IStringLocalizer _localizer;

    public GetAllAccountTransactionHandler(ApplicationDbContext context, IStringLocalizer<SharedResource> localizer)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
    }

    public async Task<PageList<AccountTransactionGetDto>> Handle(GetAllAccountTransaction request, CancellationToken cancellationToken)
    {
        var accountDetailId = await _context.Accounts
            .AsNoTracking()
            .Include(x => x.AccountDetail)
            .Where(a => a.Id == request.AccountId)
            .Select(a => a.AccountDetail.Id)
            .FirstOrDefaultAsync(cancellationToken);
        if (accountDetailId == 0)
        {
            throw new NotFoundException();
        }

        var documentItems = await _context.DocumentArticles
            .AsNoTracking()
            .Where(di => di.AccountDetailId == accountDetailId)
            .Select(di => new AccountTransactionGetDto
            {
                Id = di.Id,
                Code = di.Document.Code,
                TypeCode = di.Document.DocumentType.Code,
                TypeTitle = "",
                Credit = di.Credit,
                Debit = di.Debit,
                Date = di.Document.Date,
                Note = di.Document.Note,
            })
            .ToPagedAsync(request.Page, request.PageSize, request.SortBy);

        foreach (var item in documentItems.Items)
        {
            switch (item.TypeCode)
            {
                case "10":
                case "12":
                    item.TypeTitle = _localizer.GetString("transactionMoneyIn").Value;
                    break;
                case "11":
                case "13":
                    item.TypeTitle = _localizer.GetString("transactionMoneyOut").Value;
                    break;
                case "14":
                    item.TypeTitle = item.Credit > 0 ? _localizer.GetString("transactionTransferIn").Value : _localizer.GetString("transactionTransferOut").Value;
                    break;
                case "15":
                    item.TypeTitle = _localizer.GetString("transactionDocumentReversal").Value;
                    break;
            }
        }

        return documentItems;
    }
}