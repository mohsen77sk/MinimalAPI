using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Minimal.Api.Exceptions;
using Minimal.Api.Features.Accounts.Models;
using Minimal.DataAccess;

namespace Minimal.Api.Features.Accounts.Queries;

public class GetAccountByIdHandler : IRequestHandler<GetAccountById, AccountGetDto>
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer _localizer;

    public GetAccountByIdHandler(ApplicationDbContext context, IMapper mapper, IStringLocalizer<SharedResource> localizer)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
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
            throw new NotFoundException(_localizer.GetString("notFoundAccount").Value);
        }

        return _mapper.Map<AccountGetDto>(Account);
    }
}