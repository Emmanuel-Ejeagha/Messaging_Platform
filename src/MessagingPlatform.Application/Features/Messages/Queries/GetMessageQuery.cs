using System;
using MediatR;
using MessagingPlatform.Application.Common.Result;
using MessagingPlatform.Application.Features.Messages.DTOs;

namespace MessagingPlatform.Application.Features.Messages.Queries;

public class GetMessageQuery : IRequest<ApplicationResult<MessageDto>>
{
    public Guid ConversationId { get; set; }
    public Guid MessageId { get; set; }
}

