// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Design;
// using Microsoft.Extensions.Configuration;
// using System.IO;

// namespace MessagingPlatform.Infrastructure.Persistence;

// public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
// {
//     public ApplicationDbContext CreateDbContext(string[] args)
//     {
//         // Build configuration
//         var configuration = new ConfigurationBuilder()
//             .SetBasePath(Directory.GetCurrentDirectory())
//             .AddJsonFile("appsettings.json", optional: true)
//             .AddJsonFile($"appsettings.Development.json", optional: true)
//             .Build();

//         // Build DbContextOptions
//         var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
//         var connectionString = configuration.GetConnectionString("DefaultConnection") 
//             ?? "Host=localhost;Port=5432;Database=MessagingPlatform;Username=postgres;Password=postgres;";

//         builder.UseNpgsql(connectionString);

//         return new ApplicationDbContext(builder.Options);
//     }
// }