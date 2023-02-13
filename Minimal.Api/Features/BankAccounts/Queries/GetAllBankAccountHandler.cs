using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Minimal.Api.Extensions;
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
        var bankAccounts = _context.BankAccounts.Include(ba => ba.Bank).Include(ba => ba.Person).AsNoTracking();

        if (request.IsActive is not null)
        {
            bankAccounts = bankAccounts.Where(p => p.IsActive == request.IsActive);
        }

        return _mapper.Map<PageList<BankAccountGetDto>>(
            await bankAccounts.ToPagedAsync(request.Page, request.PageSize, request.SortBy)
        );
    }
}