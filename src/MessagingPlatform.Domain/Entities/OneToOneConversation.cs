using System;
using MessagingPlatform.Domain.Enums;
using MessagingPlatform.Domain.Exceptions;
using MessagingPlatform.Domain.ValueObjects;

namespace MessagingPlatform.Domain.Entities;

public sealed class OneToOneConversation : Conversation
{
    public override ConversationType Type => ConversationType.OneToOne;

    // Additional invariants for one-to-one conversations
    public OneToOneConversation()
    {
        // One-to-one must have exactly 2 participants
        // This is enforced by factory method
    }

    // Prevent adding more than 2 participants
    public new Participant AddParticipant(UserId userId, ParticipantRole role = ParticipantRole.Member)
    {
        if (Participants.Count >= 2)
            throw new DomainException("One-to-one conversations cannot have more than 2 participants");

        return base.AddParticipant(userId, role);
    }
}
