using System;
using AutoMapper;
using MediatR;
using MessagingPlatform.Application.Common.Interfaces;
using MessagingPlatform.Application.Common.Result;
using MessagingPlatform.Application.Features.Conversations.Commands;
using MessagingPlatform.Application.Features.Conversations.DTOs;
using MessagingPlatform.Domain.Entities;
using MessagingPlatform.Domain.Exceptions;
using MessagingPlatform.Domain.Interfaces;
using MessagingPlatform.Domain.ValueObjects;

namespace MessagingPlatform.Application.Features.Conversations.Handlers;

public class CreateConversationCommandHandler : IRequestHandler<CreateConversationCommand, ApplicationResult<ConversationDto>>
{
    private readonly IConversationRepository _conversationRepository;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateConversationCommandHandler(
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

    public async Task<ApplicationResult<ConversationDto>> Handle(CreateConversationCommand request, CancellationToken cancellationToken)
    {
        var currenrUserId = UserId.From(_currentUserService.UserId);

        try
        {
            Conversation conversation;

            switch (request.Type)
            {
                case ConversationType.OneToOne:
                    if (!request.OtherUserId.HasValue)
                        return ApplicationResult<ConversationDto>.Failure("OtherUserId is required for one-to-one conversations");

                    var otherUserId = UserId.From(request.OtherUserId.Value);

                    // Check if conversation already exists
                    var existingConversation = await _conversationRepository
                        .FindOneToOneConversationAsync(currenrUserId, otherUserId, cancellationToken);

                    if (existingConversation != null)
                        return ApplicationResult<ConversationDto>.Success(_mapper.Map<ConversationDto>(existingConversation));

                    conversation = Conversation.CreatedOneToOne(currenrUserId, otherUserId);
                    break;

                case ConversationType.Group:
                    if (string.IsNullOrWhiteSpace(request.Name))
                        return ApplicationResult<ConversationDto>.Failure("Group name is requried");

                    conversation = Conversation.CreateGroup(request.Name, currenrUserId);

                    // Add additional participants if provided
                    if (request.ParticipantIds != null)
                    {
                        foreach (var participantId in request.ParticipantIds)
                        {
                            if (participantId != _currentUserService.UserId)
                            {
                                conversation.AddParticipant(UserId.From(participantId));
                            }
                        }
                    }

                    if (conversation is GroupConversation groupConversation && !string.IsNullOrWhiteSpace(request.Description))
                    {
                        groupConversation.UpdateDescription(request.Description);
                    }
                    break;
                default:
                    return ApplicationResult<ConversationDto>.Failure("Invalid conversation type");
            }
            await _conversationRepository.AddAsync(conversation, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var conversationDto = _mapper.Map<ConversationDto>(conversation);
            return ApplicationResult<ConversationDto>.Success(conversationDto);
        }
        catch (DomainException ex)
        {
            return ApplicationResult<ConversationDto>.Failure(ex.Message);
        }
    }
}