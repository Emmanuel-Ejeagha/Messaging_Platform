using System;
using AutoMapper;
using MediatR;
using MessagingPlatform.Application.Common.Interfaces;
using MessagingPlatform.Application.Common.Result;
using MessagingPlatform.Application.Features.Conversations.DTOs;
using MessagingPlatform.Application.Features.Conversations.Queries;
using MessagingPlatform.Application.Features.Messages.DTOs;
using MessagingPlatform.Domain.Interfaces;
using MessagingPlatform.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace MessagingPlatform.Application.Features.Conversations.Handlers;

public class GetConversationDetailsQueryHandler : IRequestHandler<GetConversationDetailsQuery, ApplicationResult<ConversationDetailsDto>>
{
    private readonly IConversationRepository _conversationRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;
    private readonly ILogger<GetConversationDetailsQueryHandler> _logger;
    
    public GetConversationDetailsQueryHandler(
        IConversationRepository conversationRepository,
        ICurrentUserService currentUserService,
        IMapper mapper,
        ILogger<GetConversationDetailsQueryHandler> logger)
    {
        _conversationRepository = conversationRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
        _logger = logger;
    }
    
    public async Task<ApplicationResult<ConversationDetailsDto>> Handle(
        GetConversationDetailsQuery request, 
        CancellationToken cancellationToken)
    {
        try
        {
            var conversation = await _conversationRepository
                .GetByIdWithMessagesAsync(request.ConversationId, request.MessageLimit, cancellationToken);
            
            if (conversation == null)
                return ApplicationResult<ConversationDetailsDto>.Failure("Conversation not found");
            
            // Verify user is a participant
            var currentUserId = UserId.From(_currentUserService.UserId);
            if (!conversation.Participants.Any(p => p.UserId == currentUserId))
                return ApplicationResult<ConversationDetailsDto>.Failure("Access denied");
            
            var conversationDto = _mapper.Map<ConversationDetailsDto>(conversation);
            
            // Set recent messages
            conversationDto.RecentMessages = _mapper.Map<List<MessageDto>>(
                conversation.Messages.OrderByDescending(m => m.CreatedAt).ToList());
            
            return ApplicationResult<ConversationDetailsDto>.Success(conversationDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving conversation details {ConversationId}", request.ConversationId);
            return ApplicationResult<ConversationDetailsDto>.Failure("An error occurred while retrieving conversation details");
        }
    }
}
