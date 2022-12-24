using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Minimal.Api.Features.Banks.Models;
using Minimal.DataAccess;

namespace Minimal.Api.Features.Banks.Queries;

public class GetAllBankHandler : IRequestHandler<GetAllBank, List<BankGetDto>>
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAllBankHandler(ApplicationDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<List<BankGetDto>> Handle(GetAllBank request, CancellationToken cancellationToken)
    {
        var banks = await _context.Banks.AsNoTracking().Where(b => b.IsActive == true).ToListAsync(cancellationToken);
        return _mapper.Map<List<BankGetDto>>(banks);
    }
}