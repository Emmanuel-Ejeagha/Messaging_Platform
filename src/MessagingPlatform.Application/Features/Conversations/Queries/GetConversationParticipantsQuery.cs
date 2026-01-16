using AutoMapper;
using MediatR;
using MessagingPlatform.Application.Common.Interfaces;
using MessagingPlatform.Application.Common.Result;
using MessagingPlatform.Application.Features.Conversations.DTOs;
using MessagingPlatform.Domain.Interfaces;
using MessagingPlatform.Domain.ValueObjects;

namespace MessagingPlatform.Application.Features.Conversations.Queries;

public class GetConversationParticipantsQuery : IRequest<ApplicationResult<List<ParticipantDto>>>
{
    public Guid ConversationId { get; set; }
}

public class GetConversationParticipantsQueryHandler 
    : IRequestHandler<GetConversationParticipantsQuery, ApplicationResult<List<ParticipantDto>>>
{
    private readonly IConversationRepository _conversationRepository;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;
    
    public GetConversationParticipantsQueryHandler(
        IConversationRepository conversationRepository,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _conversationRepository = conversationRepository;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }
    
    public async Task<ApplicationResult<List<ParticipantDto>>> Handle(
        GetConversationParticipantsQuery request, 
        CancellationToken cancellationToken)
    {
        try
        {
            var conversation = await _conversationRepository
                .GetByIdAsync(request.ConversationId, cancellationToken);
            
            if (conversation == null)
                return ApplicationResult<List<ParticipantDto>>.Failure("Conversation not found");
            
            var currentUserId = UserId.From(_currentUserService.UserId);
            if (!conversation.Participants.Any(p => p.UserId == currentUserId))
                return ApplicationResult<List<ParticipantDto>>.Failure("Access denied");
            
            var participantDtos = _mapper.Map<List<ParticipantDto>>(conversation.Participants);
            return ApplicationResult<List<ParticipantDto>>.Success(participantDtos);
        }
        catch (Exception ex)
        {
            return ApplicationResult<List<ParticipantDto>>.Failure($"Error retrieving participants: {ex.Message}");
        }
    }
}