using MediatR;
using MessagingPlatform.Application.Common.Exceptions;
using MessagingPlatform.Application.Common.Interfaces;
using MessagingPlatform.Application.Messages.Dtos;
using MessagingPlatform.Domain.ValueObjects;

namespace MessagingPlatform.Application.Messages.Commands.UpdateMessage;

public record UpdateMessageCommand : IRequest<MessageDto>
{
    public Guid MessageId { get; init; }
    public Guid UserId { get; init; }
    public string Content { get; init; } = string.Empty;
}

public class UpdateMessageCommandHandler : IRequestHandler<UpdateMessageCommand, MessageDto>
{
    private readonly IMessageRepository _messageRepository;

    public UpdateMessageCommandHandler(IMessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }

    public async Task<MessageDto> Handle(
        UpdateMessageCommand request,
        CancellationToken cancellationToken)
    {
        var message = await _messageRepository.GetByIdAsync(request.MessageId, cancellationToken);

        if (message == null)
            throw new NotFoundException(nameof(Domain.Entities.Message), request.MessageId);

        // Check if user is the sender
        if (message.SenderId != request.UserId)
            throw new ForbiddenException("Only the message sender can update the message");

        // Update message content
        message.UpdateContent(request.Content, UserId.Create(request.UserId));

        _messageRepository.Update(message);

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