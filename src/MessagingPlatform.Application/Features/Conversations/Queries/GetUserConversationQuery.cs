using System;
using MediatR;
using MessagingPlatform.Application.Common.Models;
using MessagingPlatform.Application.Common.Result;
using MessagingPlatform.Application.Features.Conversations.DTOs;

namespace MessagingPlatform.Application.Features.Conversations.Queries;

public class GetUserConversationQuery : IRequest<ApplicationResult<PaginatedList<ConversationDto>>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public bool IncludeArchived { get; set; } = false;
}
