using MessagingPlatform.Domain.Entities;

namespace MessagingPlatform.Application.Common.Interfaces;

public interface IMessageRepository
{
    Task<Message?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Message>> GetByConversationIdAsync(
        Guid conversationId, 
        int page, 
        int pageSize, 
        CancellationToken cancellationToken = default);
    Task<List<Message>> GetRepliesAsync(Guid parentMessageId, CancellationToken cancellationToken = default);
    Task<int> CountByConversationIdAsync(Guid conversationId, CancellationToken cancellationToken = default);
    Task AddAsync(Message message, CancellationToken cancellationToken = default);
    void Update(Message message);
    Task<List<Message>> GetUnreadMessagesAsync(Guid userId, Guid conversationId, CancellationToken cancellationToken = default);
}