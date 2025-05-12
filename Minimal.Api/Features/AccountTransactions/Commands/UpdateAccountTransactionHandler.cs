using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Minimal.Api.Exceptions;
using Minimal.Api.Features.AccountTransactions.Models;
using Minimal.DataAccess;

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
        _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
    }

    public async Task<AccountTransactionGetDto> Handle(UpdateAccountTransaction request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var document = await _context.Documents
            .Include(d => d.DocumentType)
            .Include(d => d.DocumentItems)
            .ThenInclude(d => d.AccountDetail)
            .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);
        if (document is null)
        {
            throw new NotFoundException(_localizer.GetString("notFoundTransaction").Value);
        }

        if (document.IsActive is false)
        {
            throw new ErrorException(_localizer.GetString("transactionIsNotActive").Value);
        }

        if (document.DocumentItems.Any(x => x.AccountDetail?.IsActive == false))
        {
            throw new ErrorException(_localizer.GetString("transactionCanNotBeEdited").Value);
        }

        document.Note = request.Note;

        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<AccountTransactionGetDto>(document);
    }
}