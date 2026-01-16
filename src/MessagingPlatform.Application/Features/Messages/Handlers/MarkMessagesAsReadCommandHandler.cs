using System;
using MediatR;
using MessagingPlatform.Application.Common.Interfaces;
using MessagingPlatform.Application.Common.Result;
using MessagingPlatform.Application.Features.Messages.Commands;
using MessagingPlatform.Domain.Exceptions;
using MessagingPlatform.Domain.Interfaces;
using MessagingPlatform.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace MessagingPlatform.Application.Features.Messages.Handlers;

public class MarkMessagesAsReadCommandHandler : IRequestHandler<MarkMessagesAsReadCommand, ApplicationResult>
{
    private readonly IConversationRepository _conversationRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<MarkMessagesAsReadCommandHandler> _logger;
    
    public MarkMessagesAsReadCommandHandler(
        IConversationRepository conversationRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork,
        ILogger<MarkMessagesAsReadCommandHandler> logger)
    {
        _conversationRepository = conversationRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    
    public async Task<ApplicationResult> Handle(
        MarkMessagesAsReadCommand request, 
        CancellationToken cancellationToken)
    {
        try
        {
            var conversation = await _conversationRepository
                .GetByIdAsync(request.ConversationId, cancellationToken);
            
            if (conversation == null)
                return ApplicationResult.Failure("Conversation not found");
            
            var currentUserId = UserId.From(_currentUserService.UserId);
            
            // Verify user is a participant
            if (!conversation.Participants.Any(p => p.UserId == currentUserId))
                return ApplicationResult.Failure("Access denied");
            
            int markedCount = 0;
            
            foreach (var messageId in request.MessageIds)
            {
                try
                {
                    conversation.MarkMessageAsRead(messageId, currentUserId);
                    markedCount++;
                }
                catch (DomainException ex)
                {
                    _logger.LogDebug("Cannot mark message {MessageId} as read: {Error}", messageId, ex.Message);
                    // Continue with other messages
                }
            }
            
            if (markedCount > 0)
            {
                _conversationRepository.Update(conversation);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                
                _logger.LogInformation(
                    "User {UserId} marked {Count} messages as read in conversation {ConversationId}", 
                    currentUserId, markedCount, request.ConversationId);
            }
            
            return ApplicationResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking messages as read in conversation {ConversationId}", request.ConversationId);
            return ApplicationResult.Failure("An error occurred while marking messages as read");
        }
    }
}
