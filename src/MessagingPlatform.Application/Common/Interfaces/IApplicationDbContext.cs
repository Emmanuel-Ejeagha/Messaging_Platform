using MessagingPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace MessagingPlatform.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Conversation> Conversations { get; }
    DbSet<Message> Messages { get; }
    DbSet<Participant> Participants { get; }
    DbSet<Group> Groups { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    DatabaseFacade Database { get; }

}