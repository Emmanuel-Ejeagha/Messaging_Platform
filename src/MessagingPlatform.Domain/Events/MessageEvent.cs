using System;
using MessagingPlatform.Domain.Common;
using MessagingPlatform.Domain.Entities;
using MessagingPlatform.Domain.ValueObjects;

namespace MessagingPlatform.Domain.Events;

public sealed class MessageEditedEvent : DomainEvent
{
    public Guid MessageId { get; }
    public Guid ConversationId { get; }
    public UserId EditorId { get; }

    public MessageEditedEvent(Guid messageId, Guid conversationId, UserId editorId)
    {
        MessageId = messageId;
        ConversationId = conversationId;
        EditorId = editorId;
        AggregateId = conversationId;
        AggregateType = nameof(Conversation);
    }
}

public sealed class MessageDeletedEvent : DomainEvent
{
    public Guid MessageId { get; }
    public Guid ConversationId { get; }
    public UserId DeletedBy { get; }

    public MessageDeletedEvent(Guid messageId, Guid conversationId, UserId deletedBy)
    {
        MessageId = messageId;
        ConversationId = conversationId;
        DeletedBy = deletedBy;
        AggregateId = conversationId;
        AggregateType = nameof(Conversation);
    }
}

public sealed class MessageAddedEvent : DomainEvent
{
    public Guid ConversationId { get; set; }
    public Guid MessageId { get; set; }
    public UserId SenderId { get; set; }

    public MessageAddedEvent(Guid conversationId, Guid messageId, UserId senderId)
    {
        ConversationId = conversationId;
        MessageId = messageId;
        SenderId = senderId;
    }
}
