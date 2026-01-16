using AutoMapper;
using MediatR;
using MessagingPlatform.Application.Common.Interfaces;
using MessagingPlatform.Application.Common.Result;
using MessagingPlatform.Application.Features.Messages.Commands;
using MessagingPlatform.Application.Features.Messages.DTOs;
using MessagingPlatform.Domain.Entities;
using MessagingPlatform.Domain.Exceptions;
using MessagingPlatform.Domain.Interfaces;
using MessagingPlatform.Domain.ValueObjects;

namespace MessagingPlatform.Application.Features.Messages.Handlers;

public class EditMessageCommandHandler
    : IRequestHandler<EditMessageCommand, ApplicationResult<MessageDto>>
{
    private readonly IConversationRepository _conversationRepository;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public EditMessageCommandHandler(
        IConversationRepository conversationRepository,
        IMapper mapper,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _conversationRepository = conversationRepository;
        _mapper = mapper;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApplicationResult<MessageDto>> Handle(
        EditMessageCommand request,
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

            if (targetMessage == null || parentConversation == null)
                return ApplicationResult<MessageDto>.Failure("You cannot edit this message");

            // Check if user can edit
            if (!targetMessage.CanBeEditedBy(currentUserId))
                return ApplicationResult<MessageDto>.Failure("You cannot edit this message");

            // Update content
            var newContent = MessageContent.CreateText(request.Content);
            targetMessage.UpdateContent(newContent, currentUserId);

            _conversationRepository.Update(parentConversation);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var messageDto = _mapper.Map<MessageDto>(targetMessage);
            return ApplicationResult<MessageDto>.Success(messageDto);
        }
        catch (DomainException ex)
        {
            // TODO
            return ApplicationResult<MessageDto>.Failure(ex.Message);
        }
    }
}
