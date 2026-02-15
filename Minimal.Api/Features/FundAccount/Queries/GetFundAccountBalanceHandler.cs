using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Minimal.Api.Exceptions;
using Minimal.Api.Features.FundAccount.Models;
using Minimal.DataAccess;

namespace Minimal.Api.Features.FundAccount.Queries;

public class GetFundAccountBalanceHandler : IRequestHandler<GetFundAccountBalance, FundAccountBalanceGetDto>
{
    private readonly ApplicationDbContext _context;
    private readonly IStringLocalizer _localizer;

    public GetFundAccountBalanceHandler(ApplicationDbContext context, IStringLocalizer<SharedResource> localizer)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
    }

    public async Task<FundAccountBalanceGetDto> Handle(GetFundAccountBalance request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var FundBalance = await _context.DocumentArticles
            .AsNoTracking()
            .Where(dr => dr.Document.IsActive == true && dr.AccountSubsid.Code == "1101")
            .SumAsync(da => da.Debit - da.Credit);

        return new FundAccountBalanceGetDto { Balance = FundBalance };
    }
}