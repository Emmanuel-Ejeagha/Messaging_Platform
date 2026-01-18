using MessagingPlatform.Domain.Common;
using MessagingPlatform.Domain.Enums;
using MessagingPlatform.Domain.Events;
using MessagingPlatform.Domain.Exceptions;
using MessagingPlatform.Domain.ValueObjects;

namespace MessagingPlatform.Domain.Entities;

public sealed class Message : AggregateRoot
{
    public Guid ConversationId { get; private set; }
    public Guid SenderId { get; private set; }
    public MessageContent Content { get; private set; } = null!;
    public MessageStatus Status { get; private set; }
    public Guid? ParentMessageId { get; private set; }
    public bool IsEdited { get; private set; }
    public DateTime? ReadAt { get; private set; }
    public DateTime? DeliveredAt { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    // Navigation property for threading
    public Message? ParentMessage { get; private set; }
    private readonly List<Message> _replies = new();
    public IReadOnlyCollection<Message> Replies => _replies.AsReadOnly();

    // Private constructor for EF Core
    private Message() { }

    private Message(
        Guid conversationId,
        UserId senderId,
        MessageContent content,
        Guid? parentMessageId = null)
    {
        ConversationId = conversationId;
        SenderId = senderId.Value;
        Content = content;
        Status = MessageStatus.Sent;
        ParentMessageId = parentMessageId;
        IsEdited = false;

        AddDomainEvent(new MessageSentEvent(
            Id,
            ConversationId,
            SenderId,
            Content.Content));
    }

    public static Message Create(
        Guid conversationId,
        UserId senderId,
        string content,
        string? mediaUrl = null,
        Guid? parentMessageId = null)
    {
        var messageContent = MessageContent.Create(content, mediaUrl);
        return new Message(conversationId, senderId, messageContent, parentMessageId);
    }

    public static Message CreateReply(
        Guid conversationId,
        UserId senderId,
        Guid parentMessageId,
        string content,
        string? mediaUrl = null)
    {
        var messageContent = MessageContent.Create(content, mediaUrl);
        return new Message(conversationId, senderId, messageContent, parentMessageId);
    }

    public void UpdateContent(string newContent, UserId requesterId)
    {
        if (requesterId.Value != SenderId)
            throw new DomainException("Only the message sender can edit the message");

        if (DateTime.UtcNow > CreatedAt.AddHours(24))
            throw new DomainException("Messages can only be edited within 24 hours of sending");

        var newMessageContent = MessageContent.Create(newContent, Content.MediaUrl);
        Content = newMessageContent;
        IsEdited = true;
        SetUpdatedTimestamp();
    }

    public void MarkAsDelivered()
    {
        if (Status == MessageStatus.Sent)
        {
            Status = MessageStatus.Delivered;
            DeliveredAt = DateTime.UtcNow;
            SetUpdatedTimestamp();
        }
    }

    public void MarkAsRead()
    {
        if (Status != MessageStatus.Failed)
        {
            Status = MessageStatus.Read;
            ReadAt = DateTime.UtcNow;
            
            // Ensure delivered first
            if (!DeliveredAt.HasValue)
            {
                DeliveredAt = DateTime.UtcNow;
            }
            
            SetUpdatedTimestamp();
        }
    }

    public void MarkAsFailed()
    {
        Status = MessageStatus.Failed;
        SetUpdatedTimestamp();
    }

    public void AddReply(Message reply)
    {
        if (reply.ParentMessageId != Id)
            throw new DomainException("Reply must reference this message as parent");

        _replies.Add(reply);
    }

    public bool IsReply => ParentMessageId.HasValue;
    public void Delete(UserId requesterId)
{
    if (requesterId.Value != SenderId)
        throw new DomainException("Only the message sender can delete the message");

    IsDeleted = true;
    DeletedAt = DateTime.UtcNow;
    SetUpdatedTimestamp();
}

    public void Restore(UserId requesterId)
    {
        if (requesterId.Value != SenderId)
            throw new DomainException("Only the message sender can restore the message");

        IsDeleted = false;
        DeletedAt = null;
        SetUpdatedTimestamp();
    }
}