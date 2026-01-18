using MediatR;
using MessagingPlatform.Application.Common.Exceptions;
using MessagingPlatform.Application.Common.Interfaces;
using MessagingPlatform.Application.Conversations.Dtos;

namespace MessagingPlatform.Application.Conversations.Queries.GetConversationById;

public record GetConversationByIdQuery : IRequest<ConversationDto>
{
    public Guid ConversationId { get; init; }
    public Guid UserId { get; init; }
}

public class GetConversationByIdQueryHandler : IRequestHandler<GetConversationByIdQuery, ConversationDto>
{
    private readonly IConversationRepository _conversationRepository;

    public GetConversationByIdQueryHandler(IConversationRepository conversationRepository)
    {
        _conversationRepository = conversationRepository;
    }

    public async Task<ConversationDto> Handle(
        GetConversationByIdQuery request,
        CancellationToken cancellationToken)
    {
        var conversation = await _conversationRepository.GetByIdAsync(request.ConversationId, cancellationToken);

        if (conversation == null)
            throw new NotFoundException(nameof(Domain.Entities.Conversation), request.ConversationId);

        // Check if user is a participant
        if (!conversation.HasParticipant(Domain.ValueObjects.UserId.Create(request.UserId)))
            throw new ForbiddenException("You are not a participant in this conversation");

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