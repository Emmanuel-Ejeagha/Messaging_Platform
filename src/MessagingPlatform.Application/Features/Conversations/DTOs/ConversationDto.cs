using System;
using MessagingPlatform.Domain.Entities;

namespace MessagingPlatform.Application.Features.Conversations.DTOs;

public class ConversationDto
{
    public Guid Id { get; set; }
    public ConversationType Type { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? AvatarUrl { get; set; }
    public DateTime LastMessage { get; set; }
    public int UnreadCount { get; set; }
    public List<ParticipantDto> Participants { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}




