using System;
using MessagingPlatform.Domain.Common;
using MessagingPlatform.Domain.Entities;
using MessagingPlatform.Domain.Enums;
using MessagingPlatform.Domain.ValueObjects;

namespace MessagingPlatform.Domain.Events;

public sealed class ConversationCreatedEvent : DomainEvent
{
    public Guid ConversarionId { get; }
    public ConversationType Type { get; }

    public ConversationCreatedEvent(Guid conversationId, ConversationType type)
    {
        ConversarionId = conversationId;
        Type = type;
    }
}

public sealed class ConversationArchivedEvent : DomainEvent
{
    public Guid ConversationId { get; }

    public ConversationArchivedEvent(Guid conversationId)
    {
        ConversationId = conversationId;
        AggregateId = conversationId;
        AggregateType = nameof(Conversation);
    }
}

public sealed class ConversationDeletedEvent : DomainEvent
{
    public Guid ConversationId { get; }
    
    public ConversationDeletedEvent(Guid conversationId)
    {
        ConversationId = conversationId;
        AggregateId = conversationId;
        AggregateType = nameof(Conversation);
    }
}

