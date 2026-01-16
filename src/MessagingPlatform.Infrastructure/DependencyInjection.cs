using Microsoft.EntityFrameworkCore;
using MessagingPlatform.Application.Common.Interfaces;
using MessagingPlatform.Domain.Interfaces;
using MessagingPlatform.Infrastructure.Data;
using MessagingPlatform.Infrastructure.Data.Repositories;
using MessagingPlatform.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;



namespace MessagingPlatform.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        // Add DbContext
        services.AddDbContext<MessagingDbContext>((sp, options) =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsAssembly(typeof(MessagingDbContext).Assembly.FullName);
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorCodesToAdd: null);
            });
            
            // Enable sensitive data logging only in development
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            }
        });
        
        // Register repositories
        services.AddScoped<IConversationRepository, ConversationRepository>();
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<MessagingDbContext>());
        
        // Register services
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IDomainEventService, DomainEventService>();
        services.AddScoped<IDateTimeService, DateTimeService>();
        
        // Add HTTP context accessor for currentUserService
        services.AddHttpContextAccessor();
        
        // Add health checks
        services.AddHealthChecks()
            .AddDbContextCheck<MessagingDbContext>()
            .AddNpgSql(configuration.GetConnectionString("DefaultConnection")!);
        
        return services;
    }
}
