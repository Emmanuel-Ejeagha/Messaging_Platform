using System;
using MediatR;
using MessagingPlatform.Application.Common.Result;
using MessagingPlatform.Application.Features.Conversations.DTOs;
using MessagingPlatform.Domain.Entities;

namespace MessagingPlatform.Application.Features.Conversations.Commands;

public class CreateConversationCommand : IRequest<ApplicationResult<ConversationDto>>
{
    public ConversationType Type { get; set; }
    public Guid? OtherUserId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public List<Guid>? ParticipantIds { get; set; }
}

