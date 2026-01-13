using System;
using MessagingPlatform.Domain.Common;
using MessagingPlatform.Domain.Enums;
using MessagingPlatform.Domain.Events;
using MessagingPlatform.Domain.Exceptions;
using MessagingPlatform.Domain.ValueObjects;
using static MessagingPlatform.Domain.Exceptions.DomainException;

namespace MessagingPlatform.Domain.Entities;

public abstract class Conversation : BaseEntity, IAggregateRoot
{
    public ConversationStatus Status { get; protected set; } = ConversationStatus.Active;
    public DateTime? LastMessageAt { get; protected set; }
    
    private readonly List<Participant> _participants = new();
    public IReadOnlyCollection<Participant> Participants => _participants.AsReadOnly();

    private readonly List<Message> _messages = new();
    public IReadOnlyCollection<Message> Messages => _messages.AsReadOnly();
    public bool IsDeleted { get; protected set; } = false;
    public DateTime? DeletedAt { get; set; }

    public abstract ConversationType Type { get; }

    // Domain Methods
    public Participant AddParticipant(UserId userId, ParticipantRole role = ParticipantRole.Member)
    {
        // Check if user is already a participant
        if (_participants.Any(p => p.UserId == userId))
            throw new ParticipantDomainException($"User {userId} is already a participant");

        var participant = new Participant(Id, userId, role);
        _participants.Add(participant);

        AddDomainEvent(new ParticipantAddedEvent(Id, userId, role));
        UpdateTimestamp();

        return participant;
    }

    public Message AddMessage(
        UserId senderId,
        MessageContent content,
        Guid? parentMessageId = null,
        Dictionary<string, object>? metadata = null)
    {
        // Business rules validations
        // Validate sender is a participant
        if (!_participants.Any(p => p.UserId == senderId))
            throw new MessageDomainException($"User {senderId} is not a participant in this conversation");

        // Validate parent message exists in this conversation if provided
        if (parentMessageId.HasValue)
        {
            var parentMessage = _messages.FirstOrDefault(m => m.Id == parentMessageId);

            if (parentMessage == null)
                throw new MessageDomainException("Parent message not found in this conversation");
        }

        var message = new Message(Id, senderId, content, parentMessageId);

        if (metadata != null)
        {
            foreach (var kvp in metadata)
                message.AddMetadata(kvp.Key, kvp.Value);
        }

        _messages.Add(message);
        LastMessageAt = DateTime.UtcNow;
        UpdateTimestamp();

        AddDomainEvent(new MessageAddedEvent(Id, message.Id, senderId));

        return message;
    }

    public void Archive()
    {
        if (Status == ConversationStatus.Archived)
            return;

        Status = ConversationStatus.Archived;
        UpdateTimestamp();

        AddDomainEvent(new ConversationArchivedEvent(Id));
    }

    public void MarkMessageAsRead(Guid messageId, UserId userId)
    {
        var message = _messages.FirstOrDefault(m => m.Id == messageId);
        if (message == null)
            throw new MessageDomainException("Message not found");

        // Only mark aS readb if user is a participant and message is from someone else
        if (message.SenderId != userId)
        {
            message.MarkAsRead(userId);
            UpdateTimestamp();
        }
    }

    // Factory method for one-to-one conversations
    public static OneToOneConversation CreatedOneToOne(UserId user1Id, UserId user2Id)
    {
        var conversation = new OneToOneConversation();
        conversation.AddParticipant(user1Id, ParticipantRole.Member);
        conversation.AddParticipant(user2Id, ParticipantRole.Member);
        return conversation;
    }

    // Factory method for group conversations
    public static GroupConversation CreateGroup(string name, UserId creatorId)
    {
        var conversation = new GroupConversation(name);
        conversation.AddParticipant(creatorId, ParticipantRole.Owner);
        return conversation;
    }

    public void SoftDelete()
    {
        if (IsDeleted)
            return;

        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        UpdateTimestamp();

        AddDomainEvent(new ConversationDeletedEvent(Id));
    }
}

public enum ConversationType
{
    OneToOne,
    Group
}

