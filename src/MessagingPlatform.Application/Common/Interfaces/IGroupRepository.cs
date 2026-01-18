using MessagingPlatform.Domain.Entities;

namespace MessagingPlatform.Application.Common.Interfaces;

public interface IGroupRepository
{
    Task<Group?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Group>> GetByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default);
    Task<List<Group>> GetByMemberIdAsync(Guid memberId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Group group, CancellationToken cancellationToken = default);
    void Update(Group group);
    Task<bool> IsMemberAsync(Guid groupId, Guid userId, CancellationToken cancellationToken = default);
}