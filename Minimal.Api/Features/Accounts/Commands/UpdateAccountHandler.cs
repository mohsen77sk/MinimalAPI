using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Minimal.Api.Exceptions;
using Minimal.Api.Features.Accounts.Models;
using Minimal.DataAccess;
using Minimal.Domain;

namespace Minimal.Api.Features.Accounts.Commands;

public class UpdateAccountHandler : IRequestHandler<UpdateAccount, AccountGetDto>
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer _localizer;

    public UpdateAccountHandler(ApplicationDbContext context, IMapper mapper, IStringLocalizer<SharedResource> localizer)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _localizer = localizer ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<AccountGetDto> Handle(UpdateAccount request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var account = await _context.Accounts.Include(x => x.People).FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);
        if (account is null)
        {
            throw new NotFoundException();
        }

        var accountType = await _context.AccountTypes.FirstOrDefaultAsync(a => a.Id.Equals(request.AccountTypeId), cancellationToken);
        if (accountType is null)
        {
            throw new ValidationException(nameof(request.AccountTypeId), _localizer.GetString("notFound").Value);
        }

        // If change persons
        if (!account.People.Select(x => x.Id).ToList().SequenceEqual(request.PersonId))
        {
            // Remove persons 
            account.People.Clear();
            // Add persons
            foreach (var personId in request.PersonId)
            {
                var person = await _context.People.FirstOrDefaultAsync(a => a.Id.Equals(personId), cancellationToken);
                if (person is null)
                {
                    throw new ValidationException(nameof(request.PersonId), _localizer.GetString("notFound").Value);
                }
                account.People.Add(person);
            }
        }

        account.AccountType = accountType;
        account.CreateDate = request.CreateDate;
        account.Note = request.Note;

        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<AccountGetDto>(account);
    }
}