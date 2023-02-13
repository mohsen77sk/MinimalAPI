using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Minimal.Api.Extensions;
using Minimal.Api.Features.Banks.Models;
using Minimal.Api.Models;
using Minimal.DataAccess;

namespace Minimal.Api.Features.Banks.Queries;

public class GetAllBankHandler : IRequestHandler<GetAllBank, PageList<BankGetDto>>
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAllBankHandler(ApplicationDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<PageList<BankGetDto>> Handle(GetAllBank request, CancellationToken cancellationToken)
    {
        var banks = _context.Banks.AsNoTracking();

        if (request.IsActive is not null)
        {
            banks = banks.Where(p => p.IsActive == request.IsActive);
        }

        return _mapper.Map<PageList<BankGetDto>>(
            await banks.ToPagedAsync(request.Page, request.PageSize, request.SortBy)
        );
    }
}