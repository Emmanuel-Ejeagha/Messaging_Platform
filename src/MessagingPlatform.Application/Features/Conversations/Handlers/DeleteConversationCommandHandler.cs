using System;
using MediatR;
using MessagingPlatform.Application.Common.Interfaces;
using MessagingPlatform.Application.Common.Result;
using MessagingPlatform.Application.Features.Conversations.Commands;
using MessagingPlatform.Domain.Entities;
using MessagingPlatform.Domain.Enums;
using MessagingPlatform.Domain.Exceptions;
using MessagingPlatform.Domain.Interfaces;
using MessagingPlatform.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace MessagingPlatform.Application.Features.Conversations.Handlers;

public class DeleteConversationCommandHandler : IRequestHandler<DeleteConversationCommand, ApplicationResult>
{
    private readonly IConversationRepository _conversationRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteConversationCommandHandler> _logger;
    
    public DeleteConversationCommandHandler(
        IConversationRepository conversationRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork,
        ILogger<DeleteConversationCommandHandler> logger)
    {
        _conversationRepository = conversationRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    
    public async Task<ApplicationResult> Handle(
        DeleteConversationCommand request, 
        CancellationToken cancellationToken)
    {
        try
        {
            var conversation = await _conversationRepository
                .GetByIdAsync(request.ConversationId, cancellationToken);
            
            if (conversation == null)
                return ApplicationResult.Failure("Conversation not found");
            
            var currentUserId = UserId.From(_currentUserService.UserId);
            
            // For one-to-one conversations: either participant can delete
            // For group conversations: only owners can delete
            if (conversation is GroupConversation groupConversation)
            {
                var currentParticipant = groupConversation.Participants
                    .FirstOrDefault(p => p.UserId == currentUserId);
                
                if (currentParticipant == null || currentParticipant.Role != ParticipantRole.Owner)
                    return ApplicationResult.Failure("Only the owner can delete a group conversation");
            }
            else
            {
                // One-to-one: verify user is a participant
                if (!conversation.Participants.Any(p => p.UserId == currentUserId))
                    return ApplicationResult.Failure("You are not a participant in this conversation");
            }
            
            // Soft delete the conversation
            conversation.SoftDelete();
            
            _conversationRepository.Update(conversation);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            _logger.LogInformation(
                "Conversation {ConversationId} deleted by user {UserId}", 
                request.ConversationId, currentUserId);
            
            return ApplicationResult.Success();
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain error deleting conversation {ConversationId}", request.ConversationId);
            return ApplicationResult.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting conversation {ConversationId}", request.ConversationId);
            return ApplicationResult.Failure("An error occurred while deleting the conversation");
        }
    }
}
