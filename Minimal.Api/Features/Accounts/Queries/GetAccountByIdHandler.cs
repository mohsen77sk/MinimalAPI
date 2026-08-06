using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Minimal.Api.Exceptions;
using Minimal.Api.Features.Accounts.Models;
using Minimal.Api.Models;
using Minimal.DataAccess;

namespace Minimal.Api.Features.Accounts.Queries;

public class GetAccountByIdHandler : IRequestHandler<GetAccountById, AccountGetDto>
{
    private readonly ApplicationDbContext _context;
    private readonly IStringLocalizer _localizer;

    public GetAccountByIdHandler(ApplicationDbContext context, IStringLocalizer<SharedResource> localizer)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
    }

    public async Task<AccountGetDto> Handle(GetAccountById request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var account = await _context.Accounts
            .AsNoTracking()
            .Select(a => new AccountGetDto
            {
                Id = a.Id,
                Code = a.Code,
                AccountTypeId = a.AccountTypeId,
                AccountTypeName = a.AccountType.Name,
                Persons = a.People.Select(p => new LookupDto { Id = p.Id, Code = p.Code, Name = p.FirstName + " " + p.LastName }).ToList(),
                Balance = _context.DocumentArticles.Where(da => da.AccountDetailId == a.AccountDetail.Id).Sum(da => da.Credit - da.Debit),
                CreateDate = a.CreateDate,
                CloseDate = a.CloseDate,
                Note = a.Note,
                IsActive = a.IsActive,
            })
            .FirstOrDefaultAsync(a => a.Id == request.AccountId, cancellationToken);

        if (account is null)
        {
            throw new NotFoundException(_localizer.GetString("notFoundAccount").Value);
        }

        return account;
    }
}