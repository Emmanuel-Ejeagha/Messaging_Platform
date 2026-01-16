using System;

namespace MessagingPlatform.Domain.Common;

public abstract class BaseEntity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; protected set; }

    private readonly List<DomainEvent> _domainEvent = new();
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvent.AsReadOnly();
    protected void AddDomainEvent(DomainEvent domainEvent) => _domainEvent.Add(domainEvent);
    public void ClearDomainEvents() => _domainEvent.Clear();
    public void UpdateTimestamp() => UpdatedAt = DateTime.Now;
}

