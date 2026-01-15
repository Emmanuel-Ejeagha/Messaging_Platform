using System;
using MessagingPlatform.Domain.Common;

namespace MessagingPlatform.Application.Common.Interfaces;

public interface IDomainEventService
{
    Task Publish(DomainEvent domainEvent, CancellationToken cancellationToken = default);
}
