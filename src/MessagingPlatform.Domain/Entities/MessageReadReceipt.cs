using System;
using MessagingPlatform.Domain.Common;
using MessagingPlatform.Domain.ValueObjects;

namespace MessagingPlatform.Domain.Entities;

public sealed class MessageReadReceipt : ValueObject
{
    public Guid MessageId { get; }
    public UserId UserId { get; }
    public DateTime ReadAt { get; } = DateTime.UtcNow;

    public MessageReadReceipt(Guid messageId, UserId userId)
    {
        MessageId = messageId;
        UserId = userId;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return MessageId;
        yield return UserId;
    }
}
