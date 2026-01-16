using System;
using AutoMapper;
using MessagingPlatform.Application.Common.Interfaces;
using MessagingPlatform.Application.Common.Models;
using MessagingPlatform.Application.Common.Result;
using MessagingPlatform.Application.Features.Messages.DTOs;
using MessagingPlatform.Application.Features.Messages.Queries;
using MessagingPlatform.Domain.Interfaces;
using MessagingPlatform.Domain.ValueObjects;

namespace MessagingPlatform.Application.Features.Messages.Handlers;

public class GetConversationMessageQueryHandler
{
    private readonly IConversationRepository _conversationRepository;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public GetConversationMessageQueryHandler(
        IConversationRepository conversationRepository,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _conversationRepository = conversationRepository;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }
    
    public async Task<ApplicationResult<PaginatedList<MessageDto>>> Handle(
        GetConversationMessageQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Verify user is a participant in the conversation
            var conversation = await _conversationRepository
                .GetByIdAsync(request.ConverstionId, cancellationToken);

            if (conversation == null)
                return ApplicationResult<PaginatedList<MessageDto>>.Failure("Conversation not found");

            var currentUserid = UserId.From(_currentUserService.UserId);
            if (!conversation.Participants.Any(p => p.UserId == currentUserid))
                return ApplicationResult<PaginatedList<MessageDto>>.Failure("Access denied");

            // Get messages
            var messages = await _conversationRepository.GetConversationMessagesAsync(
                request.ConverstionId,
                (request.PageNumber - 1) * request.PageSize,
                request.PageSize,
                request.IncludeThreads,
                cancellationToken);

            // Filter by date if specific
            if (request.BeforeDate.HasValue)
            {
                messages = messages
                    .Where(m => m.CreatedAt < request.BeforeDate.Value)
                    .ToList();
            }

            var messageDtos = _mapper.Map<List<MessageDto>>(messages);

            var PaginatedList = new PaginatedList<MessageDto>(
                messageDtos,
                messageDtos.Count, // Should be total count
                request.PageNumber,
                request.PageSize);

            return ApplicationResult<PaginatedList<MessageDto>>.Success(PaginatedList);
        }
        catch (Exception ex)
        {
            return ApplicationResult<PaginatedList<MessageDto>>.Failure($"Error retrieving messages: {ex.Message}");
        }
    }
}
