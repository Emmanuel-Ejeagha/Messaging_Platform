using MediatR;
using MessagingPlatform.Application.Common.Exceptions;
using MessagingPlatform.Application.Common.Interfaces;
using MessagingPlatform.Domain.ValueObjects;

namespace MessagingPlatform.Application.Conversations.Commands.DeleteConversation;

public record DeleteConversationCommand : IRequest
{
    public Guid ConversationId { get; init; }
    public Guid UserId { get; init; }
}

public class DeleteConversationCommandHandler : IRequestHandler<DeleteConversationCommand>
{
    private readonly IConversationRepository _conversationRepository;

    public DeleteConversationCommandHandler(IConversationRepository conversationRepository)
    {
        _conversationRepository = conversationRepository;
    }

    public async Task Handle(DeleteConversationCommand request, CancellationToken cancellationToken)
    {
        var conversation = await _conversationRepository.GetByIdAsync(request.ConversationId, cancellationToken);

        if (conversation == null)
            throw new NotFoundException(nameof(Domain.Entities.Conversation), request.ConversationId);

        conversation.Delete(UserId.Create(request.UserId));
        
        _conversationRepository.Update(conversation);
    }
}