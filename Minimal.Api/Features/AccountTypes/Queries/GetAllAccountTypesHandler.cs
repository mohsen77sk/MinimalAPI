using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Minimal.Api.Extensions;
using Minimal.Api.Features.AccountTypes.Models;
using Minimal.Api.Models;
using Minimal.DataAccess;

namespace Minimal.Api.Features.AccountTypes.Queries;

public class GetAllAccountTypeHandler : IRequestHandler<GetAllAccountType, PageList<AccountTypeGetDto>>
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAllAccountTypeHandler(ApplicationDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<PageList<AccountTypeGetDto>> Handle(GetAllAccountType request, CancellationToken cancellationToken)
    {
        var accountTypes = _context.AccountTypes.AsNoTracking();

        if (request.IsActive is not null)
        {
            accountTypes = accountTypes.Where(p => p.IsActive == request.IsActive);
        }

        return _mapper.Map<PageList<AccountTypeGetDto>>(
            await accountTypes.ToPagedAsync(request.Page, request.PageSize, request.SortBy)
        );
    }
}