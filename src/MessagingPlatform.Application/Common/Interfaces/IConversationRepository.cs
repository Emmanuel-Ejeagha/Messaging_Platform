using MessagingPlatform.Domain.Entities;

namespace MessagingPlatform.Application.Common.Interfaces;

public interface IConversationRepository
{
    Task<Conversation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Conversation>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<List<Conversation>> GetByParticipantIdAsync(Guid participantId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Conversation conversation, CancellationToken cancellationToken = default);
    void Update(Conversation conversation);
    void Remove(Conversation conversation);
    Task<int> CountByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}