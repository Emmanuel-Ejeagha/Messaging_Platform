using System;
using AutoMapper;
using MediatR;
using MessagingPlatform.Application.Common.Interfaces;
using MessagingPlatform.Application.Common.Result;
using MessagingPlatform.Application.Features.Conversations.DTOs;
using MessagingPlatform.Application.Features.Conversations.Queries;
using MessagingPlatform.Domain.Interfaces;
using MessagingPlatform.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace MessagingPlatform.Application.Features.Conversations.Handlers;

public class GetConversationParticipantsQueryHandler : IRequestHandler<GetConversationParticipantsQuery, ApplicationResult<List<ParticipantDto>>>
{
    private readonly IConversationRepository _conversationRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;
    private readonly ILogger<GetConversationParticipantsQueryHandler> _logger;
    
    public GetConversationParticipantsQueryHandler(
        IConversationRepository conversationRepository,
        ICurrentUserService currentUserService,
        IMapper mapper,
        ILogger<GetConversationParticipantsQueryHandler> logger)
    {
        _conversationRepository = conversationRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
        _logger = logger;
    }
    
    public async Task<ApplicationResult<List<ParticipantDto>>> Handle(
        GetConversationParticipantsQuery request, 
        CancellationToken cancellationToken)
    {
        try
        {
            var conversation = await _conversationRepository
                .GetByIdAsync(request.ConversationId, cancellationToken);
            
            if (conversation == null)
                return ApplicationResult<List<ParticipantDto>>.Failure("Conversation not found");
            
            // Verify user is a participant
            var currentUserId = UserId.From(_currentUserService.UserId);
            if (!conversation.Participants.Any(p => p.UserId == currentUserId))
                return ApplicationResult<List<ParticipantDto>>.Failure("Access denied");
            
            var participantDtos = _mapper.Map<List<ParticipantDto>>(conversation.Participants);
            
            return ApplicationResult<List<ParticipantDto>>.Success(participantDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving participants for conversation {ConversationId}", request.ConversationId);
            return ApplicationResult<List<ParticipantDto>>.Failure("An error occurred while retrieving participants");
        }
    }
}
