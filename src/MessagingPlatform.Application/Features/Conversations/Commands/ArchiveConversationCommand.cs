using System;
using MediatR;
using MessagingPlatform.Application.Common.Result;

namespace MessagingPlatform.Application.Features.Conversations.Commands;

public class ArchiveConversationCommand : IRequest<ApplicationResult>
{
    public Guid ConversationId { get; set; }
}
