using MediatR;
using Microsoft.EntityFrameworkCore;
using Minimal.Api.Extensions;
using Minimal.Api.Features.FundAccount.Models;
using Minimal.Api.Features.FundAccount.Profiles;
using Minimal.Api.Models;
using Minimal.DataAccess;

namespace Minimal.Api.Features.FundAccount.Queries;

public class GetFundAccountTransactionsHandler : IRequestHandler<GetFundAccountTransactions, PageList<FundAccountTransactionGetDto>>
{
    private readonly ApplicationDbContext _context;
    private readonly FundAccountMapper _mapper;

    public GetFundAccountTransactionsHandler(ApplicationDbContext context, FundAccountMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<PageList<FundAccountTransactionGetDto>> Handle(GetFundAccountTransactions request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var documents = await _context.DocumentArticles
            .AsNoTracking()
            .Include(dr => dr.Document)
            .Where(dr => dr.Document.IsActive == true && dr.AccountSubsid.Code == "1101")
            .ToPagedAsync(request.Page, request.PageSize, request.SortBy);

        return _mapper.MapToPageList(documents);
    }
}