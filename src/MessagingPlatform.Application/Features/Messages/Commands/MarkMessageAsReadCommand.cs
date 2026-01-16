using MediatR;
using MessagingPlatform.Application.Common.Interfaces;
using MessagingPlatform.Application.Common.Result;
using MessagingPlatform.Domain.Exceptions;
using MessagingPlatform.Domain.Interfaces;
using MessagingPlatform.Domain.ValueObjects;

namespace MessagingPlatform.Application.Features.Messages.Commands;

public class MarkMessageAsReadCommand : IRequest<ApplicationResult>
{
    public Guid ConversationId { get; set; }
    public Guid MessageId { get; set; }
}

public class MarkMessageAsReadCommandHandler 
    : IRequestHandler<MarkMessageAsReadCommand, ApplicationResult>
{
    private readonly IConversationRepository _conversationRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;
    
    public MarkMessageAsReadCommandHandler(
        IConversationRepository conversationRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _conversationRepository = conversationRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<ApplicationResult> Handle(
        MarkMessageAsReadCommand request, 
        CancellationToken cancellationToken)
    {
        try
        {
            var conversation = await _conversationRepository
                .GetByIdAsync(request.ConversationId, cancellationToken);
            
            if (conversation == null)
                return ApplicationResult.Failure("Conversation not found");
            
            var currentUserId = UserId.From(_currentUserService.UserId);
            
            conversation.MarkMessageAsRead(request.MessageId, currentUserId);
            
            _conversationRepository.Update(conversation);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            return ApplicationResult.Success();
        }
        catch (DomainException ex)
        {
            return ApplicationResult.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            return ApplicationResult.Failure($"Error marking message as read: {ex.Message}");
        }
    }
}