using MessagingPlatform.Domain.Common;

namespace MessagingPlatform.Domain.Events;

public sealed class ConversationCreatedEvent : IDomainEvent
{
    public Guid ConversationId { get; }
    public string Title { get; }
    public List<Guid> ParticipantIds { get; }
    public DateTime OccurredOn { get; }

    public ConversationCreatedEvent(Guid conversationId, string title, List<Guid> participantIds)
    {
        ConversationId = conversationId;
        Title = title;
        ParticipantIds = participantIds;
        OccurredOn = DateTime.UtcNow;
    }
}