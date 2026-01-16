using System;
using MediatR;
using MessagingPlatform.Application.Common.Result;
using MessagingPlatform.Application.Features.Messages.DTOs;

namespace MessagingPlatform.Application.Features.Messages.Commands;

public class SendMessageCommand : IRequest<ApplicationResult<MessageDto>>
{
    public Guid ConversationId { get; set; }
    public string Content { get; set; }
    public Guid? ParentMessageId { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
}
