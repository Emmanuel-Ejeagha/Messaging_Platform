using System;
using AutoMapper;
using MediatR;
using MessagingPlatform.Application.Common.Interfaces;
using MessagingPlatform.Application.Common.Models;
using MessagingPlatform.Application.Common.Result;
using MessagingPlatform.Application.Features.Conversations.DTOs;
using MessagingPlatform.Application.Features.Conversations.Queries;
using MessagingPlatform.Domain.Exceptions;
using MessagingPlatform.Domain.Interfaces;
using MessagingPlatform.Domain.ValueObjects;

namespace MessagingPlatform.Application.Features.Conversations.Handlers;

public class GetUserConversationQueryHandler
    : IRequestHandler<GetUserConversationQuery, ApplicationResult<PaginatedList<ConversationDto>>>
{
    private readonly IConversationRepository _conversationRepository;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public GetUserConversationQueryHandler(
        IConversationRepository conversationRepository,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _conversationRepository = conversationRepository;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<ApplicationResult<PaginatedList<ConversationDto>>> Handle(
        GetUserConversationQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = UserId.From(_currentUserService.UserId);

            // Get conversations for the user
            var conversations = await _conversationRepository
                .GetUserConversationsAsync(
                    userId,
                    (request.PageNumber - 1) * request.PageSize,
                    request.PageSize,
                    cancellationToken);

            // TODO: Get total count for pagination
            //  For now, we'll return the list without total count
            // In production, implement a separate count query

            var conversationDtos = _mapper.Map<List<ConversationDto>>(conversations);

            // calculate unread count for each conversation
            foreach (var conversation in conversations)
            {
                var participant = conversation.Participants
                 .FirstOrDefault(p => p.UserId == userId);

                if (participant != null)
                {
                    var conversationDto = conversationDtos
                        .FirstOrDefault(c => c.Id == conversation.Id);

                    if (conversationDto != null)
                    {
                        conversationDto.UnreadCount = participant.UnreadCount;
                    }
                }
            }
            var paginatedList = new PaginatedList<ConversationDto>(
                conversationDtos,
                conversationDtos.Count,
                request.PageNumber,
                request.PageSize);

            return ApplicationResult<PaginatedList<ConversationDto>>.Success(paginatedList);
        }
        catch (MessageDomainException ex)
        {
            return ApplicationResult<PaginatedList<ConversationDto>>.Failure($"Error retrieving conversations: {ex.Message}");
        }
    }
}
