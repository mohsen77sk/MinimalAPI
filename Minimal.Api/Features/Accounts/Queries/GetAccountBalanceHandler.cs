using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Minimal.Api.Exceptions;
using Minimal.Api.Features.Accounts.Models;
using Minimal.DataAccess;

namespace Minimal.Api.Features.Accounts.Queries;

public class GetAccountBalanceHandler : IRequestHandler<GetAccountBalance, AccountBalanceGetDto>
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer _localizer;

    public GetAccountBalanceHandler(ApplicationDbContext context, IMapper mapper, IStringLocalizer<SharedResource> localizer)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
    }

    public async Task<AccountBalanceGetDto> Handle(GetAccountBalance request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var accountBalance = await _context.Accounts
            .AsNoTracking()
            .Include(a => a.AccountDetail)
            .ThenInclude(ad => ad.DocumentArticleList)
            .Select(a => new AccountBalanceGetDto
            {
                Id = a.Id,
                Balance = a.AccountDetail.DocumentArticleList.Where(dr => dr.Document.IsActive == true).Sum(da => da.Credit - da.Debit)
            })
            .FirstOrDefaultAsync(a => a.Id == request.AccountId, cancellationToken);

        if (accountBalance is null)
        {
            throw new NotFoundException(_localizer.GetString("notFoundAccount").Value);
        }

        return accountBalance;
    }
}