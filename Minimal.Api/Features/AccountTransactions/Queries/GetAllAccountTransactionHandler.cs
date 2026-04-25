using MediatR;
using Microsoft.EntityFrameworkCore;
using Minimal.Api.Exceptions;
using Minimal.Api.Extensions;
using Minimal.Api.Features.AccountTransactions.Models;
using Minimal.Api.Models;
using Minimal.DataAccess;

namespace Minimal.Api.Features.AccountTransactions.Queries;

public class GetAllAccountTransactionHandler : IRequestHandler<GetAllAccountTransaction, PageList<AccountTransactionGetDto>>
{
    private readonly ApplicationDbContext _context;

    public GetAllAccountTransactionHandler(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
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
                Credit = di.Credit,
                Debit = di.Debit,
                Date = di.Document.Date,
                Note = di.Document.Note,
            })
            .ToPagedAsync(request.Page, request.PageSize, request.SortBy);

        return documentItems;
    }
}