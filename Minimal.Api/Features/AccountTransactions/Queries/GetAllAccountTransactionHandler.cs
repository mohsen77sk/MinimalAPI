using MediatR;
using Microsoft.EntityFrameworkCore;
using Minimal.Api.Exceptions;
using Minimal.Api.Extensions;
using Minimal.Api.Features.AccountTransactions.Models;
using Minimal.Api.Features.AccountTransactions.Profiles;
using Minimal.Api.Models;
using Minimal.DataAccess;

namespace Minimal.Api.Features.AccountTransactions.Queries;

public class GetAllAccountTransactionHandler : IRequestHandler<GetAllAccountTransaction, PageList<AccountTransactionGetDto>>
{
    private readonly ApplicationDbContext _context;
    private readonly AccountTransactionMapper _mapper;

    public GetAllAccountTransactionHandler(ApplicationDbContext context, AccountTransactionMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
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
            .Include(di => di.Document)
            .Where(di => di.AccountDetailId == accountDetailId)
            .ToPagedAsync(request.Page, request.PageSize, request.SortBy);

        return _mapper.MapToPageList(documentItems);
    }
}