using System;
using MediatR;
using MessagingPlatform.Application.Common.Result;

namespace MessagingPlatform.Application.Features.Messages.Commands;

public class MarkMessagesAsReadCommand : IRequest<ApplicationResult>
{
    public Guid ConversationId { get; set; }
    public List<Guid> MessageIds { get; set; } = new();
}
