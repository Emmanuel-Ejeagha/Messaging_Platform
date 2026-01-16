// Domain/Entities/Message.cs
using System;
using MessagingPlatform.Domain.Common;
using MessagingPlatform.Domain.Enums;
using MessagingPlatform.Domain.Events;
using MessagingPlatform.Domain.Exceptions;
using MessagingPlatform.Domain.ValueObjects;

namespace MessagingPlatform.Domain.Entities;

public sealed class Message : BaseEntity
{
    public Guid ConversationId { get; private set; }
    public UserId SenderId { get; private set; }
    public MessageContent Content { get; private set; }  // Contains Type property
    public Guid? ParentMessageId { get; private set; }
    public int ThreadDepth { get; private set; }
    public MessageStatus Status { get; private set; } = MessageStatus.Sent;
    public bool IsDeleted { get; private set; } = false;
    public DateTime? DeletedAt { get; private set; }
    public UserId? DeletedBy { get; private set; }
    public Dictionary<string, object>? Metadata { get; private set; }

    // Read tracking
    private readonly List<MessageReadReceipt> _readReceipts = new();
    public IReadOnlyCollection<MessageReadReceipt> ReadReceipts => _readReceipts.AsReadOnly();

    // For EF Core
    private Message() { }

    public Message(Guid conversationId, UserId senderId, MessageContent content, Guid? parentMessageId = null)
    {
        ConversationId = conversationId;
        SenderId = senderId;
        Content = content;
        ParentMessageId = parentMessageId;
        ThreadDepth = parentMessageId.HasValue ? 1 : 0;
    }

    public void AddMetadata(string key, object value)
    {
        Metadata ??= new Dictionary<string, object>();
        Metadata[key] = value;
    }

    public void MarkAsRead(UserId userId)
    {
        if (_readReceipts.Any(r => r.UserId == userId))
            return;

        _readReceipts.Add(new MessageReadReceipt(Id, userId));
    }

    public void UpdateContent(MessageContent newContent, UserId editorId)
    {
        if (IsDeleted)
            throw new MessageDomainException("Cannot edit a deleted message");

        if (!CanBeEditedBy(editorId))
            throw new MessageDomainException("User cannot edit this message");
        
        Content = newContent;
        UpdateTimestamp();
        AddDomainEvent(new MessageEditedEvent(Id, ConversationId, editorId));
    }

    public void MarkAsDeleted(UserId deletedBy)
    {
        if (IsDeleted)
            return;

        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;  // ✅ Fixed typo: was DeletedBy = DeletedBy;

        Content = MessageContent.CreateDeleted("[Message deleted]");
        UpdateTimestamp();
        AddDomainEvent(new MessageDeletedEvent(Id, ConversationId, deletedBy));
    }

    public bool CanBeEditedBy(UserId userId)
    {
        return SenderId == userId &&
               !IsDeleted &&
               !_readReceipts.Any(r => r.UserId != userId);
    }

    public bool CanBeDeletedBy(UserId userId)
    {
        // Sender or conversation admin can delete
        return SenderId == userId || IsAdmin(userId);
    }
    
    private bool IsAdmin(UserId userId)
    {
        // This would require access to the participant
        // We'll implement this in the application layer or domain service
        // For now, return false
        return false;
    }
}