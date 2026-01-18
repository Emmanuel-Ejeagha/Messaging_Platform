using MediatR;
using MessagingPlatform.Application.Common.Exceptions;
using MessagingPlatform.Application.Common.Interfaces;
using MessagingPlatform.Application.Conversations.Dtos;
using MessagingPlatform.Domain.Entities;
using MessagingPlatform.Domain.Enums;
using MessagingPlatform.Domain.ValueObjects;

namespace MessagingPlatform.Application.Conversations.Commands.CreateConversation;

public record CreateConversationCommand : IRequest<ConversationDto>
{
    public string Title { get; init; } = string.Empty;
    public ConversationType Type { get; init; }
    public List<Guid> ParticipantIds { get; init; } = new();
    public Guid? GroupId { get; init; }
    public Guid CreatorId { get; init; }
}

public class CreateConversationCommandHandler : IRequestHandler<CreateConversationCommand, ConversationDto>
{
    private readonly IConversationRepository _conversationRepository;
    private readonly IGroupRepository _groupRepository;

    public CreateConversationCommandHandler(
        IConversationRepository conversationRepository,
        IGroupRepository groupRepository)
    {
        _conversationRepository = conversationRepository;
        _groupRepository = groupRepository;
    }

    public async Task<ConversationDto> Handle(
        CreateConversationCommand request,
        CancellationToken cancellationToken)
    {
        // Validate group if provided
        if (request.GroupId.HasValue)
        {
            var group = await _groupRepository.GetByIdAsync(request.GroupId.Value, cancellationToken);
            if (group == null)
                throw new NotFoundException(nameof(Group), request.GroupId.Value);

            // Ensure creator is a member of the group
            if (!group.IsMember(UserId.Create(request.CreatorId)))
                throw new ForbiddenException("You must be a member of the group to create a conversation");
        }

        // Create conversation based on type
        Conversation conversation;
        
        if (request.Type == ConversationType.OneOnOne)
        {
            if (request.ParticipantIds.Count != 2)
                throw new ValidationException("One-on-one conversations must have exactly 2 participants");

            conversation = Conversation.CreateOneOnOne(
                UserId.Create(request.ParticipantIds[0]),
                UserId.Create(request.ParticipantIds[1]));
        }
        else if (request.Type == ConversationType.Group)
        {
            if (string.IsNullOrWhiteSpace(request.Title))
                throw new ValidationException("Group conversations must have a title");

            conversation = Conversation.CreateGroup(request.Title, UserId.Create(request.CreatorId));
            
            // Add additional participants
            foreach (var participantId in request.ParticipantIds.Where(id => id != request.CreatorId))
            {
                conversation.AddParticipant(UserId.Create(participantId), ParticipantRole.Member);
            }
        }
        else
        {
            throw new ValidationException($"Invalid conversation type: {request.Type}");
        }

        // Set group reference if provided
        if (request.GroupId.HasValue)
        {
            // This would require updating the Conversation entity to support GroupId
            // We'll handle this in the Infrastructure mapping
        }

        await _conversationRepository.AddAsync(conversation, cancellationToken);

        return MapToDto(conversation);
    }

    private static ConversationDto MapToDto(Conversation conversation)
    {
        return new ConversationDto
        {
            Id = conversation.Id,
            Title = conversation.Title,
            Type = conversation.Type,
            CreatedAt = conversation.CreatedAt,
            LastMessageAt = conversation.LastMessageAt,
            IsArchived = conversation.IsArchived,
            Participants = conversation.Participants.Select(p => new ParticipantDto
            {
                UserId = p.UserId,
                Role = p.Role,
                JoinedAt = p.JoinedAt,
                LeftAt = p.LeftAt,
                IsActive = p.IsActive
            }).ToList(),
            MessageCount = conversation.MessageIds.Count
        };
    }
}