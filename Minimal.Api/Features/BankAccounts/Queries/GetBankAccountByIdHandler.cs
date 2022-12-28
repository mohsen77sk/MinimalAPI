using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Minimal.Api.Exceptions;
using Minimal.Api.Features.BankAccounts.Models;
using Minimal.DataAccess;

namespace Minimal.Api.Features.BankAccounts.Queries;

public class GetBankAccountByIdHandler : IRequestHandler<GetBankAccountById, BankAccountGetDto>
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetBankAccountByIdHandler(ApplicationDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<BankAccountGetDto> Handle(GetBankAccountById request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var BankAccount = await _context.BankAccounts
            .AsNoTracking()
            .Include(ba => ba.Bank)
            .Include(ba => ba.Person)
            .FirstOrDefaultAsync(a => a.Id == request.BankAccountId, cancellationToken);

        if (BankAccount is null)
        {
            throw new NotFoundException();
        }

        return _mapper.Map<BankAccountGetDto>(BankAccount);
    }
}