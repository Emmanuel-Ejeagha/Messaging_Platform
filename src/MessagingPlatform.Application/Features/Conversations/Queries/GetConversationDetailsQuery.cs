using MediatR;
using MessagingPlatform.Application.Common.Result;
using MessagingPlatform.Application.Features.Conversations.DTOs;

namespace MessagingPlatform.Application.Features.Conversations.Queries;

public class GetConversationDetailsQuery : IRequest<ApplicationResult<ConversationDetailsDto>>
{
    public Guid ConversationId { get; set; }
    public int MessageLimit { get; set; } = 50;
}



