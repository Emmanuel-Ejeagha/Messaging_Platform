using System;
using MediatR;
using MessagingPlatform.Application.Common.Interfaces;
using MessagingPlatform.Application.Common.Result;
using MessagingPlatform.Application.Features.Groups.Commands;
using MessagingPlatform.Domain.Entities;
using MessagingPlatform.Domain.Enums;
using MessagingPlatform.Domain.Exceptions;
using MessagingPlatform.Domain.Interfaces;
using MessagingPlatform.Domain.ValueObjects;

namespace MessagingPlatform.Application.Features.Groups.Handlers;

public class AddParticipantCommandHandler
    : IRequestHandler<AddParticipantCommand, ApplicationResult>
{
    private readonly IConversationRepository _conversationRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public AddParticipantCommandHandler(
        IConversationRepository conversationRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _conversationRepository = conversationRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApplicationResult> Handle(
        AddParticipantCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var conversation = await _conversationRepository
                .GetByIdAsync(request.ConversationId, cancellationToken);

            if (conversation == null)
                return ApplicationResult.Failure("Conversation not found");

            // Only group conversations can add participants
            if (conversation is not GroupConversation groupConversation)
                return ApplicationResult.Failure("Only group conversations can have participants added");

            var currentUserId = UserId.From(_currentUserService.UserId);
            var currentParticipant = groupConversation.Participants
                .FirstOrDefault(p => p.UserId == currentUserId);

            // Check permissions (only admins/owners can add participants)
            if (currentParticipant == null ||
                (currentParticipant.Role != ParticipantRole.Admin &&
                    currentParticipant.Role != ParticipantRole.Owner))
                return ApplicationResult.Failure("Insufficient permissions");

            var newUserId = UserId.From(request.UserId);

            // Add participant
            groupConversation.AddParticipant(newUserId, request.Role);

            // Add system message
            var systemMessage = MessageContent.CreateSystem(
                $"{_currentUserService.Username} added a participant");
            groupConversation.AddMessage(newUserId, systemMessage);

            _conversationRepository.Update(groupConversation);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ApplicationResult.Success();
        }
        catch (DomainException ex)
        {
            return ApplicationResult.Failure(ex.Message);
        }
    }
}
