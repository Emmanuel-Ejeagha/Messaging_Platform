using System;
using AutoMapper;
using MediatR;
using MessagingPlatform.Application.Common.Interfaces;
using MessagingPlatform.Application.Common.Result;
using MessagingPlatform.Application.Features.Conversations.Commands;
using MessagingPlatform.Application.Features.Conversations.DTOs;
using MessagingPlatform.Domain.Entities;
using MessagingPlatform.Domain.Enums;
using MessagingPlatform.Domain.Exceptions;
using MessagingPlatform.Domain.Interfaces;
using MessagingPlatform.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace MessagingPlatform.Application.Features.Conversations.Handlers;

public class UpdateConversationCommandHandler : IRequestHandler<UpdateConversationCommand, ApplicationResult<ConversationDto>>
{
    private readonly IConversationRepository _conversationRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateConversationCommandHandler> _logger;
    
    public UpdateConversationCommandHandler(
        IConversationRepository conversationRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<UpdateConversationCommandHandler> logger)
    {
        _conversationRepository = conversationRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }
    
    public async Task<ApplicationResult<ConversationDto>> Handle(
        UpdateConversationCommand request, 
        CancellationToken cancellationToken)
    {
        try
        {
            var conversation = await _conversationRepository
                .GetByIdAsync(request.ConversationId, cancellationToken);
            
            if (conversation == null)
                return ApplicationResult<ConversationDto>.Failure("Conversation not found");
            
            // Only group conversations can be updated with name/description/avatar
            if (conversation is not GroupConversation groupConversation)
                return ApplicationResult<ConversationDto>.Failure("Only group conversations can be updated");
            
            // Verify user is a participant
            var currentUserId = UserId.From(_currentUserService.UserId);
            var currentParticipant = groupConversation.Participants
                .FirstOrDefault(p => p.UserId == currentUserId);
            
            if (currentParticipant == null)
                return ApplicationResult<ConversationDto>.Failure("You are not a participant in this conversation");
            
            // Only admins and owners can update group details
            if (currentParticipant.Role != ParticipantRole.Admin && 
                currentParticipant.Role != ParticipantRole.Owner)
                return ApplicationResult<ConversationDto>.Failure("Only admins or owners can update group details");
            
            // Update properties if provided
            if (!string.IsNullOrEmpty(request.Name))
            {
                groupConversation.UpdateName(request.Name);
            }
            
            if (request.Description != null)
            {
                groupConversation.UpdateDescription(request.Description);
            }
            
            if (request.AvatarUrl != null)
            {
                groupConversation.UpdateAvatar(request.AvatarUrl);
            }
            
            _conversationRepository.Update(groupConversation);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            var conversationDto = _mapper.Map<ConversationDto>(groupConversation);
            
            _logger.LogInformation(
                "Group conversation {ConversationId} updated by user {UserId}", 
                request.ConversationId, currentUserId);
            
            return ApplicationResult<ConversationDto>.Success(conversationDto);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain error updating conversation {ConversationId}", request.ConversationId);
            return ApplicationResult<ConversationDto>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating conversation {ConversationId}", request.ConversationId);
            return ApplicationResult<ConversationDto>.Failure("An error occurred while updating the conversation");
        }
    }
}
