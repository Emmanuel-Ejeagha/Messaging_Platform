using MessagingPlatform.Domain.Common;
using MessagingPlatform.Domain.Enums;
using MessagingPlatform.Domain.Exceptions;
using MessagingPlatform.Domain.ValueObjects;

namespace MessagingPlatform.Domain.Entities;

public sealed class Participant : BaseEntity
{
    public Guid UserId { get; private set; }
    public Guid ConversationId { get; private set; }
    public ParticipantRole Role { get; private set; }
    public DateTime JoinedAt { get; private set; }
    public DateTime? LeftAt { get; private set; }
    public bool IsActive => LeftAt == null;

    // Navigation properties (EF Core will use these)
    public Conversation? Conversation { get; private set; }

    // Private constructor for EF Core
    private Participant() { }

    private Participant(UserId userId, ParticipantRole role, Guid conversationId)
    {
        UserId = userId.Value;
        Role = role;
        ConversationId = conversationId;
        JoinedAt = DateTime.UtcNow;
    }

    internal static Participant Create(UserId userId, ParticipantRole role, Guid conversationId)
    {
        return new Participant(userId, role, conversationId);
    }

    public void UpdateRole(ParticipantRole newRole, UserId requesterId)
    {
        // Only owners can change roles (this logic will be enforced by the aggregate root)
        Role = newRole;
        SetUpdatedTimestamp();
    }

    public void Leave()
    {
        if (LeftAt.HasValue)
            throw new DomainException("Participant has already left");

        LeftAt = DateTime.UtcNow;
        SetUpdatedTimestamp();
    }

    public void Rejoin()
    {
        if (!LeftAt.HasValue)
            throw new DomainException("Participant is already active");

        LeftAt = null;
        SetUpdatedTimestamp();
    }
}