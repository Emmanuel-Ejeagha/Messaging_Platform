using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MessagingPlatform.Application.Common.Interfaces;
using MessagingPlatform.Domain.Common;
using MessagingPlatform.Domain.Entities;
using MessagingPlatform.Infrastructure.Persistence.Configurations;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace MessagingPlatform.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public DbSet<Conversation> Conversations => Set<Conversation>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<Participant> Participants => Set<Participant>();
    public DbSet<Group> Groups => Set<Group>();

    // Explicitly implement Database property from IApplicationDbContext
    DatabaseFacade IApplicationDbContext.Database => base.Database;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from the Configurations assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Apply global query filters
        modelBuilder.Entity<Conversation>().HasQueryFilter(c => !c.IsArchived);
        modelBuilder.Entity<Participant>().HasQueryFilter(p => p.LeftAt == null);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var result = await base.SaveChangesAsync(cancellationToken);
        return result;
    }

    // We'll handle domain events differently - through the Application layer
    // This keeps the DbContext simple and focused on persistence
}