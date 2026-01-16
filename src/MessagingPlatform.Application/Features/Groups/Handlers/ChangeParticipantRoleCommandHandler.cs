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

public class ChangeParticipantRoleCommandHandler : IRequestHandler<ChangeParticipantRoleCommand, ApplicationResult>
{
    private readonly IConversationRepository _conversationRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public ChangeParticipantRoleCommandHandler(
        IConversationRepository conversationRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _conversationRepository = conversationRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApplicationResult> Handle(
        ChangeParticipantRoleCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var conversation = await _conversationRepository
                .GetByIdAsync(request.ConversationId, cancellationToken);

            if (conversation == null)
                return ApplicationResult.Failure("Conversation not found");

            if (conversation is not GroupConversation groupConversation)
                return ApplicationResult.Failure("Only group conversation support role changes");

            var currentUserId = UserId.From(_currentUserService.UserId);
            var currentParticipant = groupConversation.Participants
                .FirstOrDefault(p => p.UserId == currentUserId);

            // Only owners can change roles
            if (currentParticipant == null || currentParticipant.Role != ParticipantRole.Owner)
                return ApplicationResult.Failure("Only the owner can change the roles");

            var targetUserId = UserId.From(request.UserId);
            groupConversation.ChangeParticipantRole(targetUserId, request.NewRole);

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
