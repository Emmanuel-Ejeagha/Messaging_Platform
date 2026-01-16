using System;

namespace MessagingPlatform.Application.Features.Conversations.DTOs;

public class UpdateConversationDto
{
    public string? Name { get; set; }
    public string Description { get; set; }
    public string? AvatarUrl { get; set; }
}