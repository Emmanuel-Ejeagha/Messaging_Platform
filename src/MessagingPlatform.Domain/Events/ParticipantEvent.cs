using System;
using MessagingPlatform.Domain.Common;
using MessagingPlatform.Domain.Entities;
using MessagingPlatform.Domain.Enums;
using MessagingPlatform.Domain.ValueObjects;

namespace MessagingPlatform.Domain.Events;

public sealed class ParticipantAddedEvent : DomainEvent
{
    public Guid ConversationId { get; }
    public UserId UserId { get; }
    public ParticipantRole Role { get; }

    public ParticipantAddedEvent(Guid conversationId, UserId userId, ParticipantRole role)
    {
        ConversationId = conversationId;
        UserId = userId;
        Role = role;
    }
}

public sealed class ParticipantRoleChangedEvent : DomainEvent
{
    public Guid ConversationId { get; }
    public UserId UserId { get; }
    public ParticipantRole NewRole { get; }

    public ParticipantRoleChangedEvent(Guid conversationId, UserId userId, ParticipantRole newRole)
    {
        ConversationId = conversationId;
        UserId = userId;
        NewRole = newRole;
        AggregateId = conversationId;
        AggregateType = nameof(Conversation);
    }    

}
