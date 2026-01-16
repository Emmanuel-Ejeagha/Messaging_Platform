using System;
using MediatR;
using MessagingPlatform.Application.Common.Result;

namespace MessagingPlatform.Application.Features.Messages.Commands;

public class RemoveParticipantCommand : IRequest<ApplicationResult>
{
    public Guid ConversationId { get; set; }
    public Guid UserId { get; set; }
}
