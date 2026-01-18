using MediatR;
using MessagingPlatform.Application.Common.Exceptions;
using MessagingPlatform.Application.Common.Interfaces;
using MessagingPlatform.Application.Messages.Dtos;

namespace MessagingPlatform.Application.Messages.Queries.GetMessageById;

public record GetMessageByIdQuery : IRequest<MessageDto>
{
    public Guid MessageId { get; init; }
    public Guid UserId { get; init; }
}

public class GetMessageByIdQueryHandler : IRequestHandler<GetMessageByIdQuery, MessageDto>
{
    private readonly IMessageRepository _messageRepository;
    private readonly IConversationRepository _conversationRepository;

    public GetMessageByIdQueryHandler(
        IMessageRepository messageRepository,
        IConversationRepository conversationRepository)
    {
        _messageRepository = messageRepository;
        _conversationRepository = conversationRepository;
    }

    public async Task<MessageDto> Handle(
        GetMessageByIdQuery request,
        CancellationToken cancellationToken)
    {
        var message = await _messageRepository.GetByIdAsync(request.MessageId, cancellationToken);

        if (message == null)
            throw new NotFoundException(nameof(Domain.Entities.Message), request.MessageId);

        // Check if user is a participant in the conversation
        var conversation = await _conversationRepository.GetByIdAsync(
            message.ConversationId, 
            cancellationToken);

        if (conversation == null || !conversation.HasParticipant(Domain.ValueObjects.UserId.Create(request.UserId)))
            throw new ForbiddenException("You are not a participant in this conversation");

        return MapToDto(message);
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