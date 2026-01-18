using MediatR;
using Microsoft.EntityFrameworkCore;
using MessagingPlatform.Application.Common.Interfaces;

namespace MessagingPlatform.Application.Common.Behaviors;

public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IApplicationDbContext _dbContext;

    public TransactionBehavior(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // Only apply transactions to Commands (writes), not Queries (reads)
        if (IsNotCommand())
            return await next();

        // For simplicity, we'll rely on EF Core's default transaction behavior
        // In a real application, you might want to use explicit transactions
        // or a more sophisticated transaction strategy
        
        var response = await next();
        await _dbContext.SaveChangesAsync(cancellationToken);
        return response;
    }

    private static bool IsNotCommand()
    {
        return !typeof(TRequest).Name.EndsWith("Command");
    }
}