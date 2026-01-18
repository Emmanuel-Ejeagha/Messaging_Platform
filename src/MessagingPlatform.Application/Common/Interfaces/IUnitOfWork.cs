namespace MessagingPlatform.Application.Common.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IConversationRepository Conversations { get; }
    IMessageRepository Messages { get; }
    IGroupRepository Groups { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default);
}