using MediatR;
using Minimal.DataAccess;

namespace Minimal.Api.Behaviors;

public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;

    public TransactionBehavior(ApplicationDbContext context, ILogger<TransactionBehavior<TRequest, TResponse>> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            await _context.BeginTransactionAsync();
            var response = await next();
            await _context.CommitTransactionAsync();

            return response;
        }
        catch (Exception)
        {
            _logger.LogError("Request failed: Rolling back all the changes made to the Context");

            await _context.RollbackTransaction();
            throw;
        }
    }
}