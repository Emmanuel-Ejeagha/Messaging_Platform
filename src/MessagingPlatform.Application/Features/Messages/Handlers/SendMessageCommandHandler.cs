using System;
using AutoMapper;
using MediatR;
using MessagingPlatform.Application.Common.Interfaces;
using MessagingPlatform.Application.Common.Result;
using MessagingPlatform.Application.Features.Messages.Commands;
using MessagingPlatform.Application.Features.Messages.DTOs;
using MessagingPlatform.Domain.Exceptions;
using MessagingPlatform.Domain.Interfaces;
using MessagingPlatform.Domain.ValueObjects;

namespace MessagingPlatform.Application.Features.Messages.Handlers;

public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, ApplicationResult<MessageDto>>
{
    private readonly IConversationRepository _conversationRepository;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDomainEventService _domainEventService;

    public SendMessageCommandHandler(IConversationRepository conversationRepository,
        IMapper mapper,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork,
        IDomainEventService domainEventService)
    {
        _conversationRepository = conversationRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _domainEventService = domainEventService;
    }

    public async Task<ApplicationResult<MessageDto>> Handle(
        SendMessageCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var conversation = await _conversationRepository
            .GetByIdAsync(request.ConversationId, cancellationToken);

            if (conversation == null)
                return ApplicationResult<MessageDto>.Failure("Conversation not found");

            var currentUserId = UserId.From(_currentUserService.UserId);
            var messageContent = MessageContent.CreateText(request.Content);

            var message = conversation.AddMessage(
                currentUserId,
                messageContent,
                request.ParentMessageId,
                request.Metadata);

            //Update unread counts for all other participants
            foreach (var participant in conversation.Participants
                .Where(p => p.UserId != currentUserId))
            {
                participant.IncrementUnreadCount();
            }

            _conversationRepository.Update(conversation);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Publish domain events
            foreach (var domainEvent in conversation.DomainEvents)
            {
                await _domainEventService.Publish(domainEvent, cancellationToken);
            }

            var messageDto = _mapper.Map<MessageDto>(message);
            return ApplicationResult<MessageDto>.Success(messageDto);
        }
        catch (DomainException ex)
        {
            return ApplicationResult<MessageDto>.Failure(ex.Message);
        }
    }
}
