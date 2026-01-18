using Microsoft.EntityFrameworkCore;
using MessagingPlatform.Application.Common.Interfaces;
using MessagingPlatform.Domain.Entities;
using MessagingPlatform.Infrastructure.Persistence;

namespace MessagingPlatform.Infrastructure.Persistence.Repositories;

public class GroupRepository : IGroupRepository
{
    private readonly ApplicationDbContext _context;

    public GroupRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Group?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Groups
            .FirstOrDefaultAsync(g => g.Id == id, cancellationToken);
    }

    public async Task<List<Group>> GetByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default)
    {
        return await _context.Groups
            .Where(g => g.OwnerId == ownerId)
            .OrderByDescending(g => g.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Group>> GetByMemberIdAsync(Guid memberId, CancellationToken cancellationToken = default)
    {
        // Using JSONB containment query for PostgreSQL
        return await _context.Groups
            .Where(g => g.MemberIds.Contains(memberId))
            .OrderByDescending(g => g.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Groups
            .AnyAsync(g => g.Id == id, cancellationToken);
    }

    public async Task AddAsync(Group group, CancellationToken cancellationToken = default)
    {
        await _context.Groups.AddAsync(group, cancellationToken);
    }

    public void Update(Group group)
    {
        _context.Groups.Update(group);
    }

    public async Task<bool> IsMemberAsync(Guid groupId, Guid userId, CancellationToken cancellationToken = default)
    {
        var group = await _context.Groups
            .FirstOrDefaultAsync(g => g.Id == groupId, cancellationToken);

        return group?.MemberIds.Contains(userId) ?? false;
    }
}