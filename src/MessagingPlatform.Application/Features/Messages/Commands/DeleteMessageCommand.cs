using System;
using MediatR;
using MessagingPlatform.Application.Common.Result;

namespace MessagingPlatform.Application.Features.Messages.Commands;

public class DeleteMessageCommand
    : IRequest<ApplicationResult>
{
    public Guid MessageId { get; set; }
}
