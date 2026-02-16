using MediatR;
using Microsoft.EntityFrameworkCore;
using Minimal.Api.Extensions;
using Minimal.Api.Features.AccountTypes.Models;
using Minimal.Api.Features.AccountTypes.Profiles;
using Minimal.Api.Models;
using Minimal.DataAccess;

namespace Minimal.Api.Features.AccountTypes.Queries;

public class GetAllAccountTypeHandler : IRequestHandler<GetAllAccountType, PageList<AccountTypeGetDto>>
{
    private readonly ApplicationDbContext _context;
    private readonly AccountTypeMapper _mapper;

    public GetAllAccountTypeHandler(ApplicationDbContext context, AccountTypeMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<PageList<AccountTypeGetDto>> Handle(GetAllAccountType request, CancellationToken cancellationToken)
    {
        var accountTypes = _context.AccountTypes.AsNoTracking();

        if (request.IsActive.HasValue)
        {
            accountTypes = accountTypes.Where(p => p.IsActive == request.IsActive.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLower();
            accountTypes = accountTypes.Where(p =>
                p.Code.ToLower().Contains(search) ||
                p.Name.ToLower().Contains(search)
            );
        }

        var pagedAccountTypes = await accountTypes.ToPagedAsync(request.Page, request.PageSize, request.SortBy);

        return _mapper.MapToPageList(pagedAccountTypes);
    }
}