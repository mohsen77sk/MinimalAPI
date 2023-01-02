using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Minimal.Api.Extensions;
using Minimal.Api.Features.Accounts.Models;
using Minimal.Api.Models;
using Minimal.DataAccess;

namespace Minimal.Api.Features.Accounts.Queries;

public class GetAllAccountHandler : IRequestHandler<GetAllAccount, PageList<AccountGetDto>>
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAllAccountHandler(ApplicationDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<PageList<AccountGetDto>> Handle(GetAllAccount request, CancellationToken cancellationToken)
    {
        var Accounts = await _context.Accounts
            .AsNoTracking()
            .Include(ba => ba.AccountType)
            .Include(ba => ba.People)
            .ToPagedAsync(request.Page, request.PageSize, request.SortBy);

        return _mapper.Map<PageList<AccountGetDto>>(Accounts);
    }
}