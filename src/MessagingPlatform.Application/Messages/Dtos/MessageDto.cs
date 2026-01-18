using MessagingPlatform.Domain.Enums;

namespace MessagingPlatform.Application.Messages.Dtos;

public class MessageDto
{
    public Guid Id { get; set; }
    public Guid ConversationId { get; set; }
    public Guid SenderId { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? MediaUrl { get; set; }
    public MessageStatus Status { get; set; }
    public Guid? ParentMessageId { get; set; }
    public bool IsEdited { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? ReadAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public MessageDto? ParentMessage { get; set; }
    public List<MessageDto> Replies { get; set; } = new();
    public bool HasMedia => !string.IsNullOrEmpty(MediaUrl);
}

public class SendMessageRequest
{
    public Guid ConversationId { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? MediaUrl { get; set; }
    public Guid? ParentMessageId { get; set; }
}

public class UpdateMessageRequest
{
    public string Content { get; set; } = string.Empty;
}

public class MessagesResponse
{
    public List<MessageDto> Messages { get; set; } = new();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public bool HasNextPage { get; set; }
}

public class DeleteMessageRequest
{
    // No properties needed
}