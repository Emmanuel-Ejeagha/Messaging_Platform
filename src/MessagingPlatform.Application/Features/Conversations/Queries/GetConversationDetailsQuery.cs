using AutoMapper;
using MediatR;
using MessagingPlatform.Application.Common.Interfaces;
using MessagingPlatform.Application.Common.Result;
using MessagingPlatform.Application.Features.Conversations.DTOs;
using MessagingPlatform.Domain.Interfaces;
using MessagingPlatform.Domain.ValueObjects;

namespace MessagingPlatform.Application.Features.Conversations.Queries;

public class GetConversationDetailsQuery : IRequest<ApplicationResult<ConversationDetailsDto>>
{
    public Guid ConversationId { get; set; }
    public int MessageLimit { get; set; } = 50;
}

public class GetConversationDetailsQueryHandler 
    : IRequestHandler<GetConversationDetailsQuery, ApplicationResult<ConversationDetailsDto>>
{
    private readonly IConversationRepository _conversationRepository;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;
    
    public GetConversationDetailsQueryHandler(
        IConversationRepository conversationRepository,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _conversationRepository = conversationRepository;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }
    
    public async Task<ApplicationResult<ConversationDetailsDto>> Handle(
        GetConversationDetailsQuery request, 
        CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = UserId.From(_currentUserService.UserId);
            
            // Get conversation with messages
            var conversation = await _conversationRepository
                .GetByIdWithMessagesAsync(request.ConversationId, request.MessageLimit, cancellationToken);
            
            if (conversation == null)
                return ApplicationResult<ConversationDetailsDto>.Failure("Conversation not found");
            
            // Verify current user is a participant
            if (!conversation.Participants.Any(p => p.UserId == currentUserId))
                return ApplicationResult<ConversationDetailsDto>.Failure("Access denied");
            
            // Map to DTO
            var conversationDto = _mapper.Map<ConversationDetailsDto>(conversation);
            
            // Calculate unread count for current user
            var participant = conversation.Participants
                .FirstOrDefault(p => p.UserId == currentUserId);
            
            if (participant != null)
            {
                conversationDto.UnreadCount = participant.UnreadCount;
                
                // Update last read time (optional - could be separate endpoint)
                // participant.UpdateLastRead(DateTime.UtcNow);
            }
            
            // Order messages chronologically (most recent query returns descending)
            conversationDto.RecentMessages = conversationDto.RecentMessages
                .OrderBy(m => m.CreatedAt)
                .ToList();
            
            return ApplicationResult<ConversationDetailsDto>.Success(conversationDto);
        }
        catch (Exception ex)
        {
            return ApplicationResult<ConversationDetailsDto>.Failure(
                $"Error retrieving conversation details: {ex.Message}");
        }
    }
}