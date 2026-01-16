using System;
using AutoMapper;
using MediatR;
using MessagingPlatform.Application.Common.Interfaces;
using MessagingPlatform.Application.Common.Result;
using MessagingPlatform.Application.Features.Conversations.Queries;
using MessagingPlatform.Application.Features.Messages.DTOs;
using MessagingPlatform.Application.Features.Messages.Queries;
using MessagingPlatform.Domain.Interfaces;
using MessagingPlatform.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace MessagingPlatform.Application.Features.Messages.Handlers;

public class GetMessageQueryHandler : IRequestHandler<GetMessageQuery, ApplicationResult<MessageDto>>
{
    private readonly IConversationRepository _conversationRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;
    private readonly ILogger<GetMessageQueryHandler> _logger;
    
    public GetMessageQueryHandler(
        IConversationRepository conversationRepository,
        ICurrentUserService currentUserService,
        IMapper mapper,
        ILogger<GetMessageQueryHandler> logger)
    {
        _conversationRepository = conversationRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
        _logger = logger;
    }
    
    public async Task<ApplicationResult<MessageDto>> Handle(
        GetMessageQuery request, 
        CancellationToken cancellationToken)
    {
        try
        {
            var conversation = await _conversationRepository
                .GetByIdAsync(request.ConversationId, cancellationToken);
            
            if (conversation == null)
                return ApplicationResult<MessageDto>.Failure("Conversation not found");
            
            // Verify user is a participant
            var currentUserId = UserId.From(_currentUserService.UserId);
            if (!conversation.Participants.Any(p => p.UserId == currentUserId))
                return ApplicationResult<MessageDto>.Failure("Access denied");
            
            var message = conversation.Messages.FirstOrDefault(m => m.Id == request.MessageId);
            if (message == null)
                return ApplicationResult<MessageDto>.Failure("Message not found");
            
            var messageDto = _mapper.Map<MessageDto>(message);
            
            return ApplicationResult<MessageDto>.Success(messageDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving message {MessageId} from conversation {ConversationId}", 
                request.MessageId, request.ConversationId);
            return ApplicationResult<MessageDto>.Failure("An error occurred while retrieving the message");
        }
    }
}
