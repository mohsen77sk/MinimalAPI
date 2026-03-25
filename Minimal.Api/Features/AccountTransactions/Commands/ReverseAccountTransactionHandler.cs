using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Minimal.Api.Common.Accounting;
using Minimal.Api.Exceptions;
using Minimal.Api.Features.AccountTransactions.Models;
using Minimal.Api.Features.AccountTransactions.Profiles;
using Minimal.DataAccess;
using Minimal.Domain;

namespace Minimal.Api.Features.AccountTransactions.Commands;

public class ReverseAccountTransactionHandler : IRequestHandler<ReverseAccountTransaction, AccountTransactionGetDto>
{
    private readonly ApplicationDbContext _context;
    private readonly AccountTransactionMapper _mapper;
    private readonly IStringLocalizer _localizer;

    public ReverseAccountTransactionHandler(ApplicationDbContext context, AccountTransactionMapper mapper, IStringLocalizer<SharedResource> localizer)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
    }

    public async Task<AccountTransactionGetDto> Handle(ReverseAccountTransaction request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var originalDocument = await _context.Documents
            .Include(d => d.DocumentItems)
            .ThenInclude(d => d.AccountDetail)
            .FirstOrDefaultAsync(d => d.DocumentItems.Any(di => di.Id == request.Id), cancellationToken);
        if (originalDocument is null)
        {
            throw new NotFoundException(_localizer.GetString("notFoundTransaction").Value);
        }

        if (originalDocument.Status == DocumentStatusEnum.Reversed)
        {
            throw new ErrorException(_localizer.GetString("transactionIsReversed").Value);
        }

        if (originalDocument.DocumentItems.Any(x => x.AccountDetail?.IsActive == false))
        {
            throw new ErrorException(_localizer.GetString("transactionCanNotBeReversed").Value);
        }

        var reverseDocument = new Document
        {
            Date = originalDocument.Date.AddMinutes(1),
            Note = request.Note,
            FiscalYear = await _context.GetCurrentFiscalYearAsync(cancellationToken),
            DocumentType = await _context.GetDocumentTypeByCodeAsync("15", cancellationToken),
            DocumentItems = [],
            RefDocument = originalDocument
        };
        foreach (var detail in originalDocument.DocumentItems)
        {
            reverseDocument.DocumentItems.Add(new DocumentArticle
            {
                AccountSubsidId = detail.AccountSubsidId,
                AccountDetailId = detail.AccountDetailId,
                Credit = detail.Debit,
                Debit = detail.Credit
            });
        }
        _context.Documents.Add(reverseDocument);

        originalDocument.Status = DocumentStatusEnum.Reversed;

        await _context.SaveChangesAsync(cancellationToken);

        var accountDetailId = reverseDocument.DocumentItems.First(di => di.AccountDetail.AccountId == request.AccountId).AccountDetailId;
        return _mapper.MapToAccountTransactionGetDto(reverseDocument.DocumentItems.First(di => di.AccountDetailId == accountDetailId));
    }
}