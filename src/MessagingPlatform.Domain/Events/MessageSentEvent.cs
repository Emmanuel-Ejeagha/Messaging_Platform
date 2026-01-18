using MessagingPlatform.Domain.Common;

namespace MessagingPlatform.Domain.Events;

public sealed class MessageSentEvent : IDomainEvent
{
    public Guid MessageId { get; }
    public Guid ConversationId { get; }
    public Guid SenderId { get; }
    public string ContentPreview { get; }
    public DateTime OccurredOn { get; }

    public MessageSentEvent(Guid messageId, Guid conversationId, Guid senderId, string contentPreview)
    {
        MessageId = messageId;
        ConversationId = conversationId;
        SenderId = senderId;
        ContentPreview = contentPreview.Length > 50 ? contentPreview[..50] + "..." : contentPreview;
        OccurredOn = DateTime.UtcNow;
    }
}