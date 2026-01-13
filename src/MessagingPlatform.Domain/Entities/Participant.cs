using System;
using MessagingPlatform.Domain.Common;
using MessagingPlatform.Domain.Enums;
using MessagingPlatform.Domain.ValueObjects;

namespace MessagingPlatform.Domain.Entities;

public sealed class Participant : BaseEntity
{
    public Guid ConversationId { get; private set; }
    public UserId UserId { get; private set; }
    public ParticipantRole Role { get; private set; }
    public DateTime JoinedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? LastReadAt { get; private set; }
    public int UnreadCount { get; private set; }

    private Participant() { }

    public Participant(Guid conversationId, UserId userId, ParticipantRole role)
    {
        ConversationId = conversationId;
        UserId = userId;
        Role = role;
    }

    public void ChangeRole(ParticipantRole newRole)
    {
        Role = newRole;
        UpdateTimestamp();
    }

    public void UpdateLastRead(DateTime timestamp)
    {
        LastReadAt = timestamp;
        UnreadCount = 0;
        UpdateTimestamp();
    }

    public void IncrementUnreadCount()
    {
        UnreadCount++;
        UpdateTimestamp();
    }
    
    public void ResetUnreadCount()
    {
        UnreadCount = 0;
        UpdateTimestamp();
    }
}
