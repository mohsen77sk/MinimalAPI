using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Minimal.Api.Exceptions;
using Minimal.Api.Features.Accounts.Models;
using Minimal.DataAccess;
using Minimal.Domain;

namespace Minimal.Api.Features.Accounts.Commands;

public class CreateAccountHandler : IRequestHandler<CreateAccount, AccountGetDto>
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer _localizer;

    public CreateAccountHandler(ApplicationDbContext context, IMapper mapper, IStringLocalizer<SharedResource> localizer)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _localizer = localizer ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<AccountGetDto> Handle(CreateAccount request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var accountToAdd = _mapper.Map<Account>(request);

        var accountType = await _context.AccountTypes.FirstOrDefaultAsync(a => a.Id.Equals(request.AccountTypeId), cancellationToken);
        if (accountType is null)
        {
            throw new ValidationException(nameof(request.AccountTypeId), _localizer.GetString("notFound").Value);
        }

        foreach (var personId in request.PersonId)
        {
            var person = await _context.People.FirstOrDefaultAsync(a => a.Id.Equals(personId), cancellationToken);
            if (person is null)
            {
                throw new ValidationException(nameof(request.PersonId), _localizer.GetString("notFound").Value);
            }
            accountToAdd.People.Add(person);
        }

        accountToAdd.Code = GenerateCodeAsync();
        accountToAdd.AccountType = accountType;
        accountToAdd.IsActive = true;

        _context.Accounts.Add(accountToAdd);
        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<AccountGetDto>(accountToAdd);
    }

    /// <summary>
    /// Generate an new code for the account.
    /// </summary>
    private string GenerateCodeAsync()
    {
        int lastCode = Int32.Parse(_context.Accounts.OrderByDescending(x => x.Code).FirstOrDefault()?.Code ?? "999");
        return (lastCode + 1).ToString();
    }
}