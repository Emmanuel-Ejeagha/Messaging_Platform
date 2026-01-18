using Microsoft.EntityFrameworkCore;
using MessagingPlatform.Application.Common.Interfaces;
using MessagingPlatform.Domain.Entities;
using MessagingPlatform.Infrastructure.Persistence;

namespace MessagingPlatform.Infrastructure.Persistence.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly ApplicationDbContext _context;

    public MessageRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Message?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Messages
            .Include(m => m.Replies)
            .Include(m => m.ParentMessage)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

    public async Task<List<Message>> GetByConversationIdAsync(
        Guid conversationId, 
        int page, 
        int pageSize, 
        CancellationToken cancellationToken = default)
    {
        return await _context.Messages
            .Where(m => m.ConversationId == conversationId && m.ParentMessageId == null)
            .Include(m => m.Replies)
            .OrderByDescending(m => m.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Message>> GetRepliesAsync(Guid parentMessageId, CancellationToken cancellationToken = default)
    {
        return await _context.Messages
            .Where(m => m.ParentMessageId == parentMessageId)
            .OrderBy(m => m.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountByConversationIdAsync(Guid conversationId, CancellationToken cancellationToken = default)
    {
        return await _context.Messages
            .CountAsync(m => m.ConversationId == conversationId && m.ParentMessageId == null, cancellationToken);
    }

    public async Task AddAsync(Message message, CancellationToken cancellationToken = default)
    {
        await _context.Messages.AddAsync(message, cancellationToken);
    }

    public void Update(Message message)
    {
        _context.Messages.Update(message);
    }

    public async Task<List<Message>> GetUnreadMessagesAsync(Guid userId, Guid conversationId, CancellationToken cancellationToken = default)
    {
        return await _context.Messages
            .Where(m => m.ConversationId == conversationId && 
                   m.SenderId != userId && 
                   m.Status == Domain.Enums.MessageStatus.Delivered)
            .OrderBy(m => m.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}