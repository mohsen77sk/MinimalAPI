using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Minimal.Api.Exceptions;
using Minimal.Api.Features.Accounts.Models;
using Minimal.DataAccess;

namespace Minimal.Api.Features.Accounts.Queries;

public class GetAccountBalanceHandler : IRequestHandler<GetAccountBalance, AccountBalanceGetDto>
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAccountBalanceHandler(ApplicationDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<AccountBalanceGetDto> Handle(GetAccountBalance request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var AccountBalance = await _context.Accounts
            .AsNoTracking()
            .Include(a => a.AccountDetail)
            .ThenInclude(ad => ad.DocumentArticleList)
            .ThenInclude(da => da.Document)
            .Select(a => new AccountBalanceGetDto
            {
                Id = a.Id,
                Balance = a.AccountDetail.DocumentArticleList
                    .Where(da => da.Document.IsActive == true && da.AccountDetailId == a.AccountDetail.Id)
                    .Sum(da => da.Credit - da.Debit)
            })
            .FirstOrDefaultAsync(a => a.Id == request.AccountId, cancellationToken);

        if (AccountBalance is null)
        {
            throw new NotFoundException();
        }

        return AccountBalance;
    }
}