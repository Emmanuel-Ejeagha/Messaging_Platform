using System;
using MessagingPlatform.Domain.Entities;
using MessagingPlatform.Domain.ValueObjects;

namespace MessagingPlatform.Domain.Interfaces;

public interface IConversationRepository : IRepository<Conversation>
{
    Task<Conversation?> GetByIdWithMessagesAsync(
        Guid id, 
        int messageLimit = 50, 
        CancellationToken cancellationToken = default);
    
    Task<IEnumerable<Conversation>> GetUserConversationsAsync(
        UserId userId, 
        int skip = 0, 
        int take = 20, 
        CancellationToken cancellationToken = default);
    
    Task<OneToOneConversation?> FindOneToOneConversationAsync(
        UserId user1Id, 
        UserId user2Id, 
        CancellationToken cancellationToken = default);
    
    Task<IEnumerable<Message>> GetConversationMessagesAsync(
        Guid conversationId,
        int skip = 0,
        int take = 50,
        bool includeThreads = false,
        CancellationToken cancellationToken = default);
}
