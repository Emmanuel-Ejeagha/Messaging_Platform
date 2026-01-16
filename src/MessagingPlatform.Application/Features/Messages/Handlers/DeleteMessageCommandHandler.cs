using System;
using MediatR;
using MessagingPlatform.Application.Common.Interfaces;
using MessagingPlatform.Application.Common.Result;
using MessagingPlatform.Application.Features.Messages.Commands;
using MessagingPlatform.Domain.Entities;
using MessagingPlatform.Domain.Exceptions;
using MessagingPlatform.Domain.Interfaces;
using MessagingPlatform.Domain.ValueObjects;

namespace MessagingPlatform.Application.Features.Messages.Handlers;

public class DeleteMessageCommandHandler
    : IRequestHandler<DeleteMessageCommand, ApplicationResult>
{
    private readonly IConversationRepository _conversationRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteMessageCommandHandler(
        IConversationRepository conversationRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _conversationRepository = conversationRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApplicationResult> Handle(
        DeleteMessageCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = UserId.From(_currentUserService.UserId);
            var conversations = await _conversationRepository
                .GetUserConversationsAsync(currentUserId, 0, 1000, cancellationToken);

            Message? targetMessage = null;
            Conversation? parentConversation = null;

            foreach (var conversation in conversations)
            {
                var message = conversation.Messages.FirstOrDefault(m => m.Id == request.MessageId);
                if (message != null)
                {
                    targetMessage = message;
                    parentConversation = conversation;
                    break;
                }
            }

            // Check if user can delete
            if (targetMessage == null || parentConversation == null)
                return ApplicationResult.Failure("You cannot delete this message");

            // Mark as deleted
            targetMessage.MarkAsDeleted(currentUserId);

            _conversationRepository.Update(parentConversation);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ApplicationResult.Success();
        }
        catch (DomainException ex)
        {
            return ApplicationResult.Failure(ex.Message);
        }
    }
}
