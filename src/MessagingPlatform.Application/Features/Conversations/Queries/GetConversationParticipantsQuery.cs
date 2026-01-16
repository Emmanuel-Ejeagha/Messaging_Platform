using System;
using MediatR;
using MessagingPlatform.Application.Common.Result;
using MessagingPlatform.Application.Features.Conversations.DTOs;

namespace MessagingPlatform.Application.Features.Conversations.Queries;

public class GetConversationParticipantsQuery : IRequest<ApplicationResult<List<ParticipantDto>>>
{
    public Guid ConversationId { get; set; }
}
