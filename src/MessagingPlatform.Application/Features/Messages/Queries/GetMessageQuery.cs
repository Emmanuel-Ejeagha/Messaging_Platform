using AutoMapper;
using MediatR;
using MessagingPlatform.Application.Common.Interfaces;
using MessagingPlatform.Application.Common.Result;
using MessagingPlatform.Application.Features.Conversations.DTOs;
using MessagingPlatform.Application.Features.Messages.DTOs;
using MessagingPlatform.Domain.Exceptions;
using MessagingPlatform.Domain.Interfaces;
using MessagingPlatform.Domain.ValueObjects;

namespace MessagingPlatform.Application.Features.Messages.Queries;

public class GetMessageQuery : IRequest<ApplicationResult<MessageDto>>
{
    public Guid ConversationId { get; set; }
    public Guid MessageId { get; set; }
}

public class GetMessageQueryHandler 
    : IRequestHandler<GetMessageQuery, ApplicationResult<MessageDto>>
{
    private readonly IConversationRepository _conversationRepository;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;
    
    public GetMessageQueryHandler(
        IConversationRepository conversationRepository,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _conversationRepository = conversationRepository;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }
    
    public async Task<ApplicationResult<MessageDto>> Handle(
        GetMessageQuery request, 
        CancellationToken cancellationToken)
    {
        try
        {
            var conversation = await _conversationRepository
                .GetByIdAsync(request.ConversationId, cancellationToken);
            
            if (conversation == null)
                return ApplicationResult<MessageDto>.Failure("Conversation not found");
            
            var currentUserId = UserId.From(_currentUserService.UserId);
            if (!conversation.Participants.Any(p => p.UserId == currentUserId))
                return ApplicationResult<MessageDto>.Failure("Access denied");
            
            var message = conversation.Messages.FirstOrDefault(m => m.Id == request.MessageId);
            if (message == null || message.IsDeleted)
                return ApplicationResult<MessageDto>.Failure("Message not found");
            
            var messageDto = _mapper.Map<MessageDto>(message);
            return ApplicationResult<MessageDto>.Success(messageDto);
        }
        catch (MessageDomainException ex)
        {
            return ApplicationResult<MessageDto>.Failure($"Error retrieving message: {ex.Message}");
        }
    }
}