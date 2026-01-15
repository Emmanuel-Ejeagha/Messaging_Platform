using System;
using MessagingPlatform.Domain.Entities;
using MessagingPlatform.Domain.ValueObjects;

namespace MessagingPlatform.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default);
}
