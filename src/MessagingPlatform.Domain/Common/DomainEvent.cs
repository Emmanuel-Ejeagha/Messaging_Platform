using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MessagingPlatform.Domain.Common;

[NotMapped]
public abstract class DomainEvent //: INotification
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public Guid AggregateId { get; protected set; }
    public string AggregateType { get; protected set; } = string.Empty;

    protected DomainEvent()
    {
        // For serialization
    }
}

