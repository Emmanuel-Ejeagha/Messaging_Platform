using MessagingPlatform.Application;
using MessagingPlatform.Infrastructure;
using MessagingPlatform.API.Filters;
using Microsoft.OpenApi.Models;
using System.Reflection;
using MessagingPlatform.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ApiExceptionFilterAttribute>();
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Messaging Platform API",
        Description = "A production-ready messaging platform API"
    });

    // Include XML comments
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

// Add Application layer services
builder.Services.AddApplication();

// Add Infrastructure layer services
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Apply migrations automatically on startup
try
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        // Check if database exists and apply migrations
        if (!dbContext.Database.GetAppliedMigrations().Any())
        {
            Console.WriteLine("Applying database migrations...");
            await dbContext.Database.MigrateAsync();
            Console.WriteLine("Database migrations applied successfully!");
        }
        else
        {
            Console.WriteLine("Database already has migrations applied.");
        }
        
        // Seed sample data for development
        if (app.Environment.IsDevelopment())
        {
            Console.WriteLine("Seeding sample data...");
            await ApplicationDbContextSeed.SeedSampleDataAsync(dbContext);
            Console.WriteLine("Sample data seeded successfully!");
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Error during migration/seeding: {ex.Message}");
    throw;
}

app.Run();