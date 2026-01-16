using System;
using MediatR;
using MessagingPlatform.Application.Common.Result;
using MessagingPlatform.Domain.Enums;

namespace MessagingPlatform.Application.Features.Groups.Commands;

public class ChangeParticipantRoleCommand : IRequest<ApplicationResult>
{
    public Guid ConversationId { get; set; }
    public Guid UserId { get; set; }
    public ParticipantRole NewRole { get; set; }
}
