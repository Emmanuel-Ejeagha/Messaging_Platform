using System;

namespace MessagingPlatform.Domain.Common;

public abstract class BaseEntity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; protected set; }

    private readonly List<DomainEvent> _domainEvent = new();
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvent.AsReadOnly();
    protected void AddDomainEvent(DomainEvent domainEvent) => _domainEvent.Add(domainEvent);
    public void ClearDomainEvents() => _domainEvent.Clear();
    protected void UpdateTimestamp() => UpdatedAt = DateTime.Now;
}

public abstract class DomainEvent : INotification
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
