using System;
using MediatR;
using MessagingPlatform.Application.Common.Result;
using MessagingPlatform.Application.Features.Messages.DTOs;

namespace MessagingPlatform.Application.Features.Messages.Commands;

public class EditMessageCommand : IRequest<ApplicationResult<MessageDto>>
{
    public Guid MessageId { get; set; }
    public string Content { get; set; } = string.Empty;
}
