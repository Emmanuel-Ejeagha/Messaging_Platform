using MediatR;
using MessagingPlatform.Application.Common.Exceptions;
using MessagingPlatform.Application.Common.Interfaces;
using MessagingPlatform.Application.Common.Models;
using MessagingPlatform.Application.Messages.Dtos;

namespace MessagingPlatform.Application.Messages.Queries.GetConversationMessages;

public record GetConversationMessagesQuery : IRequest<MessagesResponse>
{
    public Guid ConversationId { get; init; }
    public Guid UserId { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public bool IncludeReplies { get; init; } = false;
}

public class GetConversationMessagesQueryHandler : IRequestHandler<GetConversationMessagesQuery, MessagesResponse>
{
    private readonly IMessageRepository _messageRepository;
    private readonly IConversationRepository _conversationRepository;

    public GetConversationMessagesQueryHandler(
        IMessageRepository messageRepository,
        IConversationRepository conversationRepository)
    {
        _messageRepository = messageRepository;
        _conversationRepository = conversationRepository;
    }

    public async Task<MessagesResponse> Handle(
        GetConversationMessagesQuery request,
        CancellationToken cancellationToken)
    {
        // Validate conversation exists and user is a participant
        var conversation = await _conversationRepository.GetByIdAsync(request.ConversationId, cancellationToken);
        if (conversation == null)
            throw new NotFoundException(nameof(Domain.Entities.Conversation), request.ConversationId);

        if (!conversation.HasParticipant(Domain.ValueObjects.UserId.Create(request.UserId)))
            throw new ForbiddenException("You are not a participant in this conversation");

        // Get messages
        var messages = await _messageRepository.GetByConversationIdAsync(
            request.ConversationId,
            request.Page,
            request.PageSize,
            cancellationToken);

        // Get total count
        var totalCount = await _messageRepository.CountByConversationIdAsync(
            request.ConversationId,
            cancellationToken);

        // Map to DTOs
        var messageDtos = new List<MessageDto>();
        foreach (var message in messages)
        {
            var dto = MapToDto(message);

            if (request.IncludeReplies && message.Replies.Any())
            {
                dto.Replies = message.Replies.Select(MapToDto).ToList();
            }

            messageDtos.Add(dto);
        }

        return new MessagesResponse
        {
            Messages = messageDtos,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            HasNextPage = request.Page * request.PageSize < totalCount
        };
    }

    private static MessageDto MapToDto(Domain.Entities.Message message)
    {
        return new MessageDto
        {
            Id = message.Id,
            ConversationId = message.ConversationId,
            SenderId = message.SenderId,
            Content = message.Content.Content,
            MediaUrl = message.Content.MediaUrl,
            Status = message.Status,
            ParentMessageId = message.ParentMessageId,
            IsEdited = message.IsEdited,
            CreatedAt = message.CreatedAt,
            UpdatedAt = message.UpdatedAt,
            ReadAt = message.ReadAt,
            DeliveredAt = message.DeliveredAt
        };
    }
}