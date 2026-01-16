// Infrastructure/Data/ConfigurationTest.cs
using MessagingPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public static class ConfigurationTest
{
    public static void TestAllConfigurations()
    {
        var services = new ServiceCollection();
        
        services.AddDbContext<MessagingDbContext>(options =>
            options.UseNpgsql("Host=localhost;Database=Test;Username=postgres;Password=postgres"));
        
        var serviceProvider = services.BuildServiceProvider();
        
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<MessagingDbContext>();
        
        try
        {
            // This will validate the entire model
            context.Database.EnsureCreated();
            Console.WriteLine("✅ All configurations are valid!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Configuration error: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
            }
        }
    }
}