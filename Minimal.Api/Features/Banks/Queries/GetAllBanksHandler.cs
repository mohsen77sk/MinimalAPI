using MediatR;
using Microsoft.EntityFrameworkCore;
using Minimal.Api.Extensions;
using Minimal.Api.Features.Banks.Models;
using Minimal.Api.Features.Banks.Profiles;
using Minimal.Api.Models;
using Minimal.DataAccess;

namespace Minimal.Api.Features.Banks.Queries;

public class GetAllBankHandler : IRequestHandler<GetAllBank, PageList<BankGetDto>>
{
    private readonly ApplicationDbContext _context;
    private readonly BankMapper _mapper;

    public GetAllBankHandler(ApplicationDbContext context, BankMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<PageList<BankGetDto>> Handle(GetAllBank request, CancellationToken cancellationToken)
    {
        var banks = _context.Banks.AsNoTracking();

        if (request.IsActive.HasValue)
        {
            banks = banks.Where(p => p.IsActive == request.IsActive.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLower();
            banks = banks.Where(b =>
                b.Code.ToLower().Contains(search) ||
                b.Name.ToLower().Contains(search)
            );
        }

        var pagedBanks = await banks.ToPagedAsync(request.Page, request.PageSize, request.SortBy);

        return _mapper.MapToPageList(pagedBanks);
    }
}