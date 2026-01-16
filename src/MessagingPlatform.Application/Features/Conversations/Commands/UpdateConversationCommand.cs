using System;
using MediatR;
using MessagingPlatform.Application.Common.Result;
using MessagingPlatform.Application.Features.Conversations.DTOs;

namespace MessagingPlatform.Application.Features.Conversations.Commands;

public class UpdateConversationCommand : IRequest<ApplicationResult<ConversationDto>>
{
    public Guid ConversationId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? AvatarUrl { get; set; }
}

