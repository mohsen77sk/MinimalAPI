using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Minimal.Api.Exceptions;
using Minimal.Api.Features.AccountTransactions.Models;
using Minimal.DataAccess;
using Minimal.Domain;

namespace Minimal.Api.Features.AccountTransactions.Commands;

public class UpdateAccountTransactionHandler : IRequestHandler<UpdateAccountTransaction, AccountTransactionGetDto>
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer _localizer;

    public UpdateAccountTransactionHandler(ApplicationDbContext context, IMapper mapper, IStringLocalizer<SharedResource> localizer)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _localizer = localizer ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<AccountTransactionGetDto> Handle(UpdateAccountTransaction request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var account = await _context.Accounts
            .AsNoTracking()
            .Include(a => a.AccountDetail)
            .FirstOrDefaultAsync(a => a.Id == request.AccountId, cancellationToken);
        if (account is null)
        {
            throw new NotFoundException();
        }

        if (account.IsActive is false)
        {
            throw new ValidationException(nameof(request.AccountId), _localizer.GetString("accountIsNotActive").Value);
        }

        if (account.CreateDate > request.Date)
        {
            throw new ValidationException(nameof(request.Date), _localizer.GetString("transactionDateIsEarlierOpeningDate").Value);
        }

        var document = await _context.Documents
            .Include(d => d.DocumentType)
            .FirstOrDefaultAsync(a => a.Id == request.AccountId, cancellationToken);
        if (document is null)
        {
            throw new NotFoundException();
        }

        if (!new[] { "12", "13" }.Contains(document.DocumentType.Code))
        {
            throw new ValidationException(nameof(request.Date), _localizer.GetString("transactionCannotBeChanged").Value);
        }

        document.Date = request.Date;
        document.Note = request.Note;
        document.DocumentType = request.TransactionType == TransactionTypeEnum.Credit ?
            await _context.DocumentTypes.SingleAsync(at => at.Code == "12", cancellationToken) :
            await _context.DocumentTypes.SingleAsync(at => at.Code == "13", cancellationToken);
        var da1 = document.DocumentItems.Single(da => da.AccountDetailId == account.AccountDetail.Id);
        da1.Credit = request.TransactionType == TransactionTypeEnum.Credit ? request.Amount : 0;
        da1.Debit = request.TransactionType == TransactionTypeEnum.Debit ? request.Amount : 0;
        var da2 = document.DocumentItems.Single(da => da.AccountDetailId != account.AccountDetail.Id);
        da2.Credit = request.TransactionType == TransactionTypeEnum.Credit ? request.Amount : 0;
        da2.Debit = request.TransactionType == TransactionTypeEnum.Debit ? request.Amount : 0;

        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<AccountTransactionGetDto>(document);
    }
}