using System;
using MessagingPlatform.Domain.Interfaces;

namespace MessagingPlatform.Infrastructure.Data;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly MessagingDbContext _dbContext;

    public UnitOfWork(MessagingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveEntitiesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}
