using System;
using System.Threading.Tasks;
using MessagingPlatform.Application.Common.Interfaces;
using MessagingPlatform.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MessagingPlatform.Infrastructure.Data;

public class MessagingDbContextFactory : IDesignTimeDbContextFactory<MessagingDbContext>
{
    public MessagingDbContext CreateDbContext(string[] args)
    {
        Console.WriteLine("Creating DbContext for design-time...");
        
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
            ?? "Host=localhost;Database=MessagingPlatform;Username=postgres;Password=postgres";
        
        Console.WriteLine($"Connection string: {connectionString}");

        var optionsBuilder = new DbContextOptionsBuilder<MessagingDbContext>();
        
        optionsBuilder.UseNpgsql(connectionString, options =>
        {
            options.MigrationsAssembly(typeof(MessagingDbContext).Assembly.FullName);
        });

        return new MessagingDbContext(
            optionsBuilder.Options,
            new DesignTimeCurrentUserService(),
            new DesignTimeDomainEventService());
    }
}

public class DesignTimeCurrentUserService : ICurrentUserService
{
    public Guid UserId => Guid.NewGuid();
    public bool IsAuthenticated => true;
    public string? Email => "migration@example.com";
    public string? Username => "migration-user";
}

public class DesignTimeDomainEventService : IDomainEventService
{
    public Task Publish(DomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}