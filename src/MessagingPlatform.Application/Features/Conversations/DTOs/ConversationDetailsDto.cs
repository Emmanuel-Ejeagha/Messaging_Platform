using System;
using MessagingPlatform.Application.Features.Messages.DTOs;

namespace MessagingPlatform.Application.Features.Conversations.DTOs;

public class ConversationDetailsDto : ConversationDto
{
    public List<MessageDto> RecentMessages { get; set; } = new();
}
