using MediatR;
using MessagingPlatform.Application.Common.Exceptions;
using MessagingPlatform.Application.Common.Interfaces;
using MessagingPlatform.Application.Conversations.Dtos;
using MessagingPlatform.Domain.ValueObjects;

namespace MessagingPlatform.Application.Conversations.Commands.UpdateConversation;

public record UpdateConversationCommand : IRequest<ConversationDto>
{
    public Guid ConversationId { get; init; }
    public Guid UserId { get; init; }
    public string? Title { get; init; }
    public bool? IsArchived { get; init; }
}

public class UpdateConversationCommandHandler : IRequestHandler<UpdateConversationCommand, ConversationDto>
{
    private readonly IConversationRepository _conversationRepository;

    public UpdateConversationCommandHandler(IConversationRepository conversationRepository)
    {
        _conversationRepository = conversationRepository;
    }

    public async Task<ConversationDto> Handle(
        UpdateConversationCommand request,
        CancellationToken cancellationToken)
    {
        var conversation = await _conversationRepository.GetByIdAsync(request.ConversationId, cancellationToken);

        if (conversation == null)
            throw new NotFoundException(nameof(Domain.Entities.Conversation), request.ConversationId);

        if (!conversation.HasParticipant(UserId.Create(request.UserId)))
            throw new ForbiddenException("You are not a participant in this conversation");

        // Update title if provided
        if (request.Title != null && conversation.Type == Domain.Enums.ConversationType.Group)
        {
            conversation.UpdateTitle(request.Title, UserId.Create(request.UserId));
        }
        else if (request.Title != null && conversation.Type == Domain.Enums.ConversationType.OneOnOne)
        {
            throw new ValidationException("One-on-one conversations cannot have their title updated");
        }

        // Update archive status if provided
        if (request.IsArchived.HasValue)
        {
            if (request.IsArchived.Value)
                conversation.Archive();
            else
                conversation.Unarchive();
        }

        _conversationRepository.Update(conversation);

        return MapToDto(conversation);
    }

    private static ConversationDto MapToDto(Domain.Entities.Conversation conversation)
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