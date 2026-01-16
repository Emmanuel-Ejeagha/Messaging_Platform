using System;

namespace MessagingPlatform.Application.Features.Conversations.DTOs;

public class ParticipantDto
{
    public Guid UserId { get; set; }
    public string Role { get; set; } = string.Empty;
    public DateTime JoinedAt { get; set; }
    public DateTime? LastReadAt { get; set; }
}
