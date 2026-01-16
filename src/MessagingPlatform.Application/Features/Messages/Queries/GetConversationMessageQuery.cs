using System;
using MediatR;
using MessagingPlatform.Application.Common.Models;
using MessagingPlatform.Application.Common.Result;
using MessagingPlatform.Application.Features.Messages.DTOs;

namespace MessagingPlatform.Application.Features.Messages.Queries;

public class GetConversationMessageQuery : IRequest<ApplicationResult<PaginatedList<MessageDto>>>
{
    public Guid ConverstionId { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public bool IncludeThreads { get; set; } = false;
    public DateTime? BeforeDate { get; set; }
}
