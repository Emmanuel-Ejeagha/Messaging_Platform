using MediatR;
using MessagingPlatform.Domain.Enums;
using MessagingPlatform.Domain.ValueObjects;

namespace MessagingPlatform.Application.Events;

public record MessageAddedNotification(Guid ConversationId, Guid MessageId, UserId SenderId) : INotification;
public record MessageEditedNotification(Guid MessageId, Guid ConversationId, UserId EditorId) : INotification;
public record MessageDeletedNotification(Guid MessageId, Guid ConversationId, UserId DeletedBy) : INotification;
public record ParticipantAddedNotification(Guid ConversationId, UserId UserId, ParticipantRole Role) : INotification;
