using System;

namespace MessagingPlatform.Application.Features.Messages.DTOs;

public class SendMessageDto
{
    public Guid ConversationId { get; set; }
    public string Content { get; set; } = string.Empty;
    public Guid? ParentMessageId { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
}
