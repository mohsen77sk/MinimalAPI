using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Minimal.Api.Exceptions;
using Minimal.Api.Features.Accounts.Models;
using Minimal.DataAccess;

namespace Minimal.Api.Features.Accounts.Queries;

public class GetAccountByIdHandler : IRequestHandler<GetAccountById, AccountGetDto>
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAccountByIdHandler(ApplicationDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<AccountGetDto> Handle(GetAccountById request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var Account = await _context.Accounts
            .AsNoTracking()
            .Include(a => a.AccountType)
            .Include(a => a.People)
            .FirstOrDefaultAsync(a => a.Id == request.AccountId, cancellationToken);

        if (Account is null)
        {
            throw new NotFoundException();
        }

        return _mapper.Map<AccountGetDto>(Account);
    }
}