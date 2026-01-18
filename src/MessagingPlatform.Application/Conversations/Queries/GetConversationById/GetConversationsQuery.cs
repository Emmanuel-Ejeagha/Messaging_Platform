using MediatR;
using MessagingPlatform.Application.Common.Interfaces;
using MessagingPlatform.Application.Conversations.Dtos;

namespace MessagingPlatform.Application.Conversations.Queries.GetConversations;

public record GetConversationsQuery : IRequest<List<ConversationDto>>
{
    public Guid UserId { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

public class GetConversationsQueryHandler : IRequestHandler<GetConversationsQuery, List<ConversationDto>>
{
    private readonly IConversationRepository _conversationRepository;

    public GetConversationsQueryHandler(IConversationRepository conversationRepository)
    {
        _conversationRepository = conversationRepository;
    }

    public async Task<List<ConversationDto>> Handle(
        GetConversationsQuery request,
        CancellationToken cancellationToken)
    {
        var conversations = await _conversationRepository.GetByUserIdAsync(
            request.UserId,
            cancellationToken);

        // Simple pagination (in a real app, you'd implement proper pagination in the repository)
        var paginatedConversations = conversations
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        return paginatedConversations.Select(MapToDto).ToList();
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