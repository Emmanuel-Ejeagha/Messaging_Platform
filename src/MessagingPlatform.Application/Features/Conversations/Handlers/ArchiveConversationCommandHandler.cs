using System;
using MediatR;
using MessagingPlatform.Application.Common.Interfaces;
using MessagingPlatform.Application.Common.Result;
using MessagingPlatform.Application.Features.Conversations.Commands;
using MessagingPlatform.Domain.Exceptions;
using MessagingPlatform.Domain.Interfaces;
using MessagingPlatform.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace MessagingPlatform.Application.Features.Conversations.Handlers;

public class ArchiveConversationCommandHandler : IRequestHandler<ArchiveConversationCommand, ApplicationResult>
{
    private readonly IConversationRepository _conversationRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ArchiveConversationCommandHandler> _logger;
    
    public ArchiveConversationCommandHandler(
        IConversationRepository conversationRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork,
        ILogger<ArchiveConversationCommandHandler> logger)
    {
        _conversationRepository = conversationRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    
    public async Task<ApplicationResult> Handle(
        ArchiveConversationCommand request, 
        CancellationToken cancellationToken)
    {
        try
        {
            var conversation = await _conversationRepository
                .GetByIdAsync(request.ConversationId, cancellationToken);
            
            if (conversation == null)
                return ApplicationResult.Failure("Conversation not found");
            
            // Verify user is a participant
            var currentUserId = UserId.From(_currentUserService.UserId);
            if (!conversation.Participants.Any(p => p.UserId == currentUserId))
                return ApplicationResult.Failure("You are not a participant in this conversation");
            
            // Archive the conversation
            conversation.Archive();
            
            _conversationRepository.Update(conversation);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            _logger.LogInformation(
                "Conversation {ConversationId} archived by user {UserId}", 
                request.ConversationId, currentUserId);
            
            return ApplicationResult.Success();
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain error archiving conversation {ConversationId}", request.ConversationId);
            return ApplicationResult.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error archiving conversation {ConversationId}", request.ConversationId);
            return ApplicationResult.Failure("An error occurred while archiving the conversation");
        }
    }
}
