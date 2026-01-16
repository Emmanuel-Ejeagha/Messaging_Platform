using MediatR;
using Microsoft.Extensions.Logging;

namespace MessagingPlatform.Application.Events.Handlers;

public class MessageAddedNotificationHandler : INotificationHandler<MessageAddedNotification>
{
    private readonly ILogger<MessageAddedNotificationHandler> _logger;
    
    public MessageAddedNotificationHandler(ILogger<MessageAddedNotificationHandler> logger)
    {
        _logger = logger;
    }
    
    public Task Handle(MessageAddedNotification notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Message {MessageId} added to conversation {ConversationId} by user {SenderId}",
            notification.MessageId, notification.ConversationId, notification.SenderId);
        
        // Here you could:
        // 1. Update read/unread counts
        // 2. Send notifications to other participants
        // 3. Update conversation last message timestamp
        // 4. Trigger push notifications (when we add real-time)
        
        return Task.CompletedTask;
    }
}