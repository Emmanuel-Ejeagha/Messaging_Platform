using MediatR;
using MessagingPlatform.Application.Common.Exceptions;
using MessagingPlatform.Application.Common.Interfaces;
using MessagingPlatform.Application.Messages.Dtos;
using MessagingPlatform.Domain.Entities;
using MessagingPlatform.Domain.ValueObjects;

namespace MessagingPlatform.Application.Messages.Commands.SendMessage;

public record SendMessageCommand : IRequest<MessageDto>
{
    public Guid ConversationId { get; init; }
    public Guid SenderId { get; init; }
    public string Content { get; init; } = string.Empty;
    public string? MediaUrl { get; init; }
    public Guid? ParentMessageId { get; init; }
}

public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, MessageDto>
{
    private readonly IMessageRepository _messageRepository;
    private readonly IConversationRepository _conversationRepository;

    public SendMessageCommandHandler(
        IMessageRepository messageRepository,
        IConversationRepository conversationRepository)
    {
        _messageRepository = messageRepository;
        _conversationRepository = conversationRepository;
    }

    public async Task<MessageDto> Handle(
        SendMessageCommand request,
        CancellationToken cancellationToken)
    {
        // Validate conversation exists and user is a participant
        var conversation = await _conversationRepository.GetByIdAsync(request.ConversationId, cancellationToken);
        if (conversation == null)
            throw new NotFoundException(nameof(Conversation), request.ConversationId);

        if (!conversation.HasParticipant(UserId.Create(request.SenderId)))
            throw new ForbiddenException("You are not a participant in this conversation");

        // Validate parent message if provided
        if (request.ParentMessageId.HasValue)
        {
            var parentMessage = await _messageRepository.GetByIdAsync(request.ParentMessageId.Value, cancellationToken);
            if (parentMessage == null)
                throw new NotFoundException(nameof(Message), request.ParentMessageId.Value);

            if (parentMessage.ConversationId != request.ConversationId)
                throw new ValidationException("Parent message must be in the same conversation");
        }

        // Create message
        Message message;
        if (request.ParentMessageId.HasValue)
        {
            message = Message.CreateReply(
                request.ConversationId,
                UserId.Create(request.SenderId),
                request.ParentMessageId.Value,
                request.Content,
                request.MediaUrl);
        }
        else
        {
            message = Message.Create(
                request.ConversationId,
                UserId.Create(request.SenderId),
                request.Content,
                request.MediaUrl);
        }

        // Add message to conversation
        conversation.AddMessage(message.Id);

        // Save message
        await _messageRepository.AddAsync(message, cancellationToken);

        return MapToDto(message);
    }

    private static MessageDto MapToDto(Message message)
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