using System;
using MessagingPlatform.Domain.Entities;
using MessagingPlatform.Domain.Interfaces;
using MessagingPlatform.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace MessagingPlatform.Infrastructure.Data.Repositories;

public class ConversationRepository : IConversationRepository
{
    private readonly MessagingDbContext _dbContext;

    public IUnitOfWork UnitOfWork => _dbContext;

    public ConversationRepository(MessagingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Conversation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Conversations
            .Include(c => c.Participants)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Conversation?> GetByIdWithMessagesAsync(
        Guid id,
        int messageLimit = 50,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Conversations
            .Include(c => c.Participants)
            .Include(c => c.Messages
                .Where(m => !m.IsDeleted)
                .OrderByDescending(m => m.CreatedAt)
                .Take(messageLimit))
            .ThenInclude(m => m.ReadReceipts)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Conversation>> GetUserConversationsAsync(
        UserId userId,
        int skip = 0,
        int take = 20,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Conversations
            .Include(c => c.Participants)
            .Include(c => c.Messages
                .Where(m => !m.IsDeleted)
                .OrderByDescending(m => m.CreatedAt)
                .Take(1)) // Get last message for preview
            .Where(c => c.Participants.Any(p => p.UserId == userId))
            .Where(c => !c.IsDeleted)
            .OrderByDescending(c => c.LastMessageAt ?? c.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<OneToOneConversation?> FindOneToOneConversationAsync(
        UserId user1Id,
        UserId user2Id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Conversations
            .OfType<OneToOneConversation>()
            .Include(c => c.Participants)
            .Where(c => c.Participants.Any(p => p.UserId == user1Id) &&
                        c.Participants.Any(p => p.UserId == user2Id))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<Message>> GetConversationMessagesAsync(
        Guid conversationId,
        int skip = 0,
        int take = 50,
        bool includeThreads = false,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Messages
            .Include(m => m.ReadReceipts)
            .Where(m => m.ConversationId == conversationId && !m.IsDeleted);

        if (!includeThreads)
        {
            // Only get root messages (no parent)
            query = query.Where(m => m.ParentMessageId == null);
        }

        return await query
            .OrderByDescending(m => m.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Conversation conversation, CancellationToken cancellationToken = default)
    {
        await _dbContext.Conversations.AddAsync(conversation, cancellationToken);
    }

    public void Update(Conversation conversation)
    {
        _dbContext.Conversations.Update(conversation);
    }

    public async Task<IEnumerable<Conversation>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Conversations
            .Include(c => c.Participants)
            .ToListAsync(cancellationToken);
    }

    public void Delete(Conversation entity)
    {
        // Soft delete by setting IsDeleted flag
        entity.SoftDelete();
        _dbContext.Conversations.Update(entity);
    }
}
