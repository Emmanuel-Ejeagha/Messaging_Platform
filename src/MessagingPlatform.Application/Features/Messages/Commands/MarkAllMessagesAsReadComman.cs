using MediatR;
using MessagingPlatform.Application.Common.Interfaces;
using MessagingPlatform.Application.Common.Result;
using MessagingPlatform.Domain.Interfaces;
using MessagingPlatform.Domain.ValueObjects;

namespace MessagingPlatform.Application.Features.Messages.Commands;

public class MarkAllMessagesAsReadCommand : IRequest<ApplicationResult>
{
    public Guid ConversationId { get; set; }
}

public class MarkAllMessagesAsReadCommandHandler 
    : IRequestHandler<MarkAllMessagesAsReadCommand, ApplicationResult>
{
    private readonly IConversationRepository _conversationRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;
    
    public MarkAllMessagesAsReadCommandHandler(
        IConversationRepository conversationRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _conversationRepository = conversationRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<ApplicationResult> Handle(
        MarkAllMessagesAsReadCommand request, 
        CancellationToken cancellationToken)
    {
        try
        {
            var conversation = await _conversationRepository
                .GetByIdAsync(request.ConversationId, cancellationToken);
            
            if (conversation == null)
                return ApplicationResult.Failure("Conversation not found");
            
            var currentUserId = UserId.From(_currentUserService.UserId);
            var participant = conversation.Participants
                .FirstOrDefault(p => p.UserId == currentUserId);
            
            if (participant == null)
                return ApplicationResult.Failure("Access denied");
            
            // Update participant's last read time
            participant.UpdateLastRead(DateTime.UtcNow);
            
            // Mark all messages as read
            foreach (var message in conversation.Messages.Where(m => !m.IsDeleted))
            {
                message.MarkAsRead(currentUserId);
            }
            
            _conversationRepository.Update(conversation);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            return ApplicationResult.Success();
        }
        catch (Exception ex)
        {
            return ApplicationResult.Failure($"Error marking all messages as read: {ex.Message}");
        }
    }
}