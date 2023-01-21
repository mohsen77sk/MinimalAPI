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

        var account = await _context.Accounts
            .Include(a => a.People)
            .Include(a => a.AccountDetail)
            .ThenInclude(ad => ad.DocumentArticleList)
            .ThenInclude(da => da.Document)
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);
        if (account is null)
        {
            throw new NotFoundException();
        }

        // Select the type of account opening document 
        var openingDocumentType = await _context.DocumentTypes.SingleAsync(dt => dt.Code == "10", cancellationToken);
        // Selecting the account opening document
        var openingDocument = account.AccountDetail.DocumentArticleList.Single(da => da.Document.DocumentTypeId == openingDocumentType.Id).Document;

        // If change account type
        if (account.AccountTypeId != request.AccountTypeId)
        {
            var accountType = await _context.AccountTypes.FirstOrDefaultAsync(at => at.Id.Equals(request.AccountTypeId), cancellationToken);
            if (accountType is null)
            {
                throw new ValidationException(nameof(request.AccountTypeId), _localizer.GetString("notFound").Value);
            }
            account.AccountType = accountType;
            account.AccountDetail.Code = accountType.Code + account.Code;
            openingDocument.DocumentItems.Single(dr => dr.AccountDetailId == account.AccountDetail.Id).AccountSubsid
                = await _context.AccountSubsids.SingleAsync(x => x.Code == accountType.Code, cancellationToken);
        }

        // If change create date
        if (account.CreateDate != request.CreateDate)
        {
            if (account.AccountDetail.DocumentArticleList.Any(dr =>
                dr.Document.IsActive == true && dr.Document.DocumentTypeId != openingDocumentType.Id && dr.Document.Date <= request.CreateDate))
            {
                throw new ValidationException(nameof(request.CreateDate), _localizer.GetString("openingAccountDateIsAfterTransaction").Value);
            }

            account.CreateDate = request.CreateDate;
            openingDocument.Date = request.CreateDate;
        }

        // If change init credit
        if (openingDocument.DocumentItems.Sum(dr => dr.Credit) != request.InitCredit)
        {
            openingDocument.DocumentItems.Single(dr => dr.AccountDetailId == account.AccountDetail.Id).Credit = request.InitCredit;
            openingDocument.DocumentItems.Single(dr => dr.AccountDetailId != account.AccountDetail.Id).Debit = request.InitCredit;
        }

        // If change persons
        if (!account.People.Select(p => p.Id).ToList().SequenceEqual(request.PersonId))
        {
            // Remove persons 
            account.People.Clear();
            // Add persons
            foreach (var personId in request.PersonId)
            {
                var person = await _context.People.FirstOrDefaultAsync(p => p.Id.Equals(personId), cancellationToken);
                if (person is null)
                {
                    throw new ValidationException(nameof(request.PersonId), _localizer.GetString("notFound").Value);
                }
                if (person.IsActive is false)
                {
                    throw new ValidationException(nameof(request.PersonId), _localizer.GetString("personIsNotActive").Value);
                }
                account.People.Add(person);
            }
        }

        account.Note = request.Note;

        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<AccountGetDto>(account);
    }
}