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
        var account = await _context.Accounts
            .AsNoTracking()
            .Include(x => x.AccountDetail)
            .Select(a => new
            {
                Id = a.Id,
                AccountDetailId = a.AccountDetail.Id
            })
            .FirstOrDefaultAsync(a => a.Id == request.AccountId, cancellationToken);
        if (account is null)
        {
            throw new NotFoundException();
        }

        var documents = await _context.Documents
            .AsNoTracking()
            .Include(d => d.DocumentType)
            .Include(d => d.DocumentItems)
            .ThenInclude(di => di.AccountDetail)
            .Where(d => d.IsActive == true && d.DocumentItems.Any(x => x.AccountDetailId == account.AccountDetailId))
            .ToPagedAsync(request.Page, request.PageSize, request.SortBy);

        return _mapper.MapToPageList(documents);
    }
}