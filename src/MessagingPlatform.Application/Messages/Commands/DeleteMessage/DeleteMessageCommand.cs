using MediatR;
using MessagingPlatform.Application.Common.Exceptions;
using MessagingPlatform.Application.Common.Interfaces;

namespace MessagingPlatform.Application.Messages.Commands.DeleteMessage;

public record DeleteMessageCommand : IRequest
{
    public Guid MessageId { get; init; }
    public Guid UserId { get; init; }
}

public class DeleteMessageCommandHandler : IRequestHandler<DeleteMessageCommand>
{
    private readonly IMessageRepository _messageRepository;

    public DeleteMessageCommandHandler(IMessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }

    public async Task Handle(
        DeleteMessageCommand request,
        CancellationToken cancellationToken)
    {
        var message = await _messageRepository.GetByIdAsync(request.MessageId, cancellationToken);

        if (message == null)
            throw new NotFoundException(nameof(Domain.Entities.Message), request.MessageId);

        // Check if user is the sender or has admin rights
        // For now, only sender can delete
        if (message.SenderId != request.UserId)
            throw new ForbiddenException("Only the message sender can delete the message");

        // In a real application, you might want to soft delete
        // For now, we'll just remove it from the repository
        // _messageRepository.Remove(message);
        
        // Instead of hard delete, let's mark as deleted or archive
        // Since we don't have that field, we'll just throw for now
        throw new NotImplementedException("Message deletion is not yet implemented. Consider soft delete.");
    }
}