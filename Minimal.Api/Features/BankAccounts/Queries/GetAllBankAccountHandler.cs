using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Minimal.Api.Extension;
using Minimal.Api.Features.BankAccounts.Models;
using Minimal.Api.Models;
using Minimal.DataAccess;

namespace Minimal.Api.Features.BankAccounts.Queries;

public class GetAllBankAccountHandler : IRequestHandler<GetAllBankAccount, PageList<BankAccountGetDto>>
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAllBankAccountHandler(ApplicationDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<PageList<BankAccountGetDto>> Handle(GetAllBankAccount request, CancellationToken cancellationToken)
    {
        var bankAccounts = await _context.BankAccounts
            .AsNoTracking()
            .Include(ba => ba.Bank)
            .Include(ba => ba.Person)
            .ToPagedAsync(request.Page, request.PageSize, request.SortBy);

        return _mapper.Map<PageList<BankAccountGetDto>>(bankAccounts);
    }
}