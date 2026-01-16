using System;
using System.Threading.Tasks;
using MessagingPlatform.Application.Common.Interfaces;
using MessagingPlatform.Domain.Common;
using MessagingPlatform.Domain.Entities;
using MessagingPlatform.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MessagingPlatform.Infrastructure.Data;

public class MessagingDbContext : DbContext, IUnitOfWork
{
    private readonly ICurrentUserService? _currentUserService;
    private readonly IDomainEventService? _domainEventService;

    public DbSet<Conversation> Conversations => Set<Conversation>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<Participant> Participants => Set<Participant>();

    // Parameterless constructor for design-time
    public MessagingDbContext() { }

    // Main constructor for runtime
    public MessagingDbContext(
        DbContextOptions<MessagingDbContext> options,
        ICurrentUserService currentUserService,
        IDomainEventService domainEventService) : base(options)
    {
        _currentUserService = currentUserService;
        _domainEventService = domainEventService;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // Only configure for design-time
            var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
                ?? "Host=localhost;Database=MessagingPlatform;Username=postgres;Password=postgres";
            
            optionsBuilder.UseNpgsql(connectionString, npgsqlOptions => 
            {
                npgsqlOptions.MigrationsAssembly(typeof(MessagingDbContext).Assembly.FullName);
            });
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Apply all configurations from assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MessagingDbContext).Assembly);
        
        // Configure global query filters
        ConfigureGlobalFilters(modelBuilder);
    }

    private static void ConfigureGlobalFilters(ModelBuilder modelBuilder)
    {
        // Soft delete filter for conversations
        modelBuilder.Entity<Conversation>()
            .HasQueryFilter(c => !c.IsDeleted);

        // Soft delete filter for messages
        modelBuilder.Entity<Message>()
            .HasQueryFilter(m => !m.IsDeleted);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Update audit fields
        UpdateAuditableEntities();
        
        // Dispatch domain events before saving
        await DispatchDomainEventsAsync();
        
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateAuditableEntities()
    {
        var entries = ChangeTracker.Entries<BaseEntity>();
        
        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
            }
            
            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdateTimestamp();
            }
        }
    }

    private async Task DispatchDomainEventsAsync()
    {
        if (_domainEventService == null) return;
        
        var entitiesWithEvents = ChangeTracker.Entries<BaseEntity>()
            .Select(e => e.Entity)
            .Where(e => e.DomainEvents.Any())
            .ToArray();
        
        foreach (var entity in entitiesWithEvents)
        {
            var events = entity.DomainEvents.ToArray();
            entity.ClearDomainEvents();
            
            foreach (var domainEvent in events)
            {
                await _domainEventService.Publish(domainEvent);
            }
        }
    }
    
    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        var result = await SaveChangesAsync(cancellationToken);
        return result > 0;
    }
}