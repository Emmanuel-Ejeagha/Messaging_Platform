using System;
using MediatR;
using MessagingPlatform.Application.Common.Interfaces;
using MessagingPlatform.Domain.Common;
using MessagingPlatform.Domain.Events;
using Microsoft.Extensions.Logging;

namespace MessagingPlatform.Infrastructure.Services;

public class DomainEventService : IDomainEventService
{
    private readonly IMediator _mediator;
    private readonly ILogger<DomainEventService> _logger;
    private readonly IPublisher _publisher;

    public DomainEventService(IMediator mediator, ILogger<DomainEventService> logger, IPublisher publisher)
    {
        _mediator = mediator;
        _logger = logger;
        _publisher = publisher;
    }

    public async Task Publish(DomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Publishing domain event: {EventName} - {EventId}",
            domainEvent.GetType().Name, domainEvent.EventId);


        // Convert domain event to MediatR notification
        var notification = CreateNotification(domainEvent);

        if (notification != null)
        {
            await _mediator.Publish(notification, cancellationToken);
        }

        // publish to outbox for reliable messaging (if implemented)
        await PublishToOutbox(domainEvent, cancellationToken);
    }

    private INotification? CreateNotification(DomainEvent domainEvent)
    {
        // Map domain events to MediaR notifications
        return domainEvent switch
        {
            MessageAddedEvent messageAdded => new Application.Events.MessageAddedNotification(
                messageAdded.ConversationId,
                messageAdded.MessageId,
                messageAdded.SenderId),
            MessageEditedEvent messageEdited => new Application.Events.MessageEditedNotification(
                messageEdited.MessageId,
                messageEdited.ConversationId,
                messageEdited.EditorId),
            MessageDeletedEvent messageDeleted => new Application.Events.MessageDeletedNotification(
                messageDeleted.MessageId,
                messageDeleted.ConversationId,
                messageDeleted.DeletedBy),
            ParticipantAddedEvent participantAdded => new Application.Events.ParticipantAddedNotification(
                participantAdded.ConversationId,
                participantAdded.UserId,
                participantAdded.Role),
            _ => null
        };
    }
    
    private async Task PublishToOutbox(DomainEvent domainEvent, CancellationToken cancellationToken)
    {
        // Implementation for outbox pattern
        // This would store the event in a database table for reliable delivery
        // For now, we'll log it
        _logger.LogDebug("Event would be added to outbox: {EventType}", domainEvent.GetType().Name);
    }
}
