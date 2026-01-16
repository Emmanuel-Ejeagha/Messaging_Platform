using AutoMapper;
using MediatR;
using MessagingPlatform.Application.Common.Interfaces;
using MessagingPlatform.Application.Common.Result;
using MessagingPlatform.Application.Features.Messages.DTOs;
using MessagingPlatform.Domain.Entities;
using MessagingPlatform.Domain.Interfaces;
using MessagingPlatform.Domain.ValueObjects;

namespace MessagingPlatform.Application.Features.Messages.Queries;

public class GetMessageThreadQuery : IRequest<ApplicationResult<List<MessageDto>>>
{
    public Guid ConversationId { get; set; }
    public Guid MessageId { get; set; }
    public int MaxDepth { get; set; } = 10; // Prevent infinite recursion
}

public class GetMessageThreadQueryHandler 
    : IRequestHandler<GetMessageThreadQuery, ApplicationResult<List<MessageDto>>>
{
    private readonly IConversationRepository _conversationRepository;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;
    
    public GetMessageThreadQueryHandler(
        IConversationRepository conversationRepository,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _conversationRepository = conversationRepository;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }
    
    public async Task<ApplicationResult<List<MessageDto>>> Handle(
        GetMessageThreadQuery request, 
        CancellationToken cancellationToken)
    {
        try
        {
            var conversation = await _conversationRepository
                .GetByIdAsync(request.ConversationId, cancellationToken);
            
            if (conversation == null)
                return ApplicationResult<List<MessageDto>>.Failure("Conversation not found");
            
            var currentUserId = UserId.From(_currentUserService.UserId);
            if (!conversation.Participants.Any(p => p.UserId == currentUserId))
                return ApplicationResult<List<MessageDto>>.Failure("Access denied");
            
            // Find the root message
            var rootMessage = conversation.Messages.FirstOrDefault(m => m.Id == request.MessageId);
            if (rootMessage == null || rootMessage.IsDeleted)
                return ApplicationResult<List<MessageDto>>.Failure("Message not found");
            
            // Get all messages in the thread (this is simplified)
            // In production, we'd use a recursive query or closure table
            var threadMessages = GetThreadMessagesRecursive(conversation.Messages, rootMessage.Id, request.MaxDepth);
            
            var messageDtos = _mapper.Map<List<MessageDto>>(threadMessages);
            return ApplicationResult<List<MessageDto>>.Success(messageDtos);
        }
        catch (Exception ex)
        {
            return ApplicationResult<List<MessageDto>>.Failure($"Error retrieving message thread: {ex.Message}");
        }
    }
    
    private List<Message> GetThreadMessagesRecursive(
        IReadOnlyCollection<Message> allMessages, 
        Guid parentMessageId, 
        int maxDepth,
        int currentDepth = 0)
    {
        if (currentDepth >= maxDepth)
            return new List<Message>();
        
        var result = new List<Message>();
        var childMessages = allMessages
            .Where(m => m.ParentMessageId == parentMessageId && !m.IsDeleted)
            .ToList();
        
        foreach (var child in childMessages)
        {
            result.Add(child);
            result.AddRange(GetThreadMessagesRecursive(allMessages, child.Id, maxDepth, currentDepth + 1));
        }
        
        return result;
    }
}