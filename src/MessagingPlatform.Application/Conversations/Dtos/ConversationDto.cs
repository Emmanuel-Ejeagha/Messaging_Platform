using MessagingPlatform.Application.Messages.Dtos;
using MessagingPlatform.Domain.Enums;

namespace MessagingPlatform.Application.Conversations.Dtos;

public class ConversationDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public ConversationType Type { get; set; }
    public Guid? GroupId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastMessageAt { get; set; }
    public bool IsArchived { get; set; }
    public List<ParticipantDto> Participants { get; set; } = new();
    public int MessageCount { get; set; }
    public MessageDto? LastMessage { get; set; }
}

public class ParticipantDto
{
    public Guid UserId { get; set; }
    public ParticipantRole Role { get; set; }
    public DateTime JoinedAt { get; set; }
    public DateTime? LeftAt { get; set; }
    public bool IsActive { get; set; }
}

public class CreateConversationRequest
{
    public string Title { get; set; } = string.Empty;
    public ConversationType Type { get; set; }
    public List<Guid> ParticipantIds { get; set; } = new();
    public Guid? GroupId { get; set; }
}

public class UpdateConversationRequest
{
    public string? Title { get; set; }
    public bool? IsArchived { get; set; }
}

public class AddParticipantRequest
{
    public Guid UserId { get; set; }
}

public class DeleteConversationRequest
{
    // No properties needed, just the command
}