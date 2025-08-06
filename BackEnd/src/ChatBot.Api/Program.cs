using ChatBot.Application;
using ChatBot.Infrastructure;
using Serilog;
using ChatBot.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using ChatBot.Api.Extensions;
using ChatBot.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSignalR();

builder.Services.AddHostedService<InactiveSessionCleanupService>();

builder.Services.AddApiServices(builder.Configuration);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<ChatBot.Api.Middleware.ExceptionHandlingMiddleware>();

// Database initialization with improved error handling
{
    using (var scope = app.Services.CreateScope()) 
    {
        var services = scope.ServiceProvider;
        try
        {
            var dbContext = services.GetRequiredService<ChatBotDbContext>();
            app.Logger.LogInformation("Attempting to connect to database and apply migrations...");
            
            // Ensure database is created
            var canConnect = await dbContext.Database.CanConnectAsync();
            if (!canConnect)
            {
                app.Logger.LogInformation("Database does not exist. Creating database...");
                await dbContext.Database.EnsureCreatedAsync();
                app.Logger.LogInformation("Database created successfully.");
            }
            else
            {
                app.Logger.LogInformation("Database connection established successfully.");
            }
            
            // Apply migrations
            var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
            {
                app.Logger.LogInformation("Applying {Count} pending migrations...", pendingMigrations.Count());
                await dbContext.Database.MigrateAsync();
                app.Logger.LogInformation("Database migrations applied successfully.");
            }
            else
            {
                app.Logger.LogInformation("Database is up to date. No migrations to apply.");
            }
        }
        catch (Exception ex)
        {
            app.Logger.LogError(ex, "An error occurred while initializing the database. Connection String: {ConnectionString}", 
                builder.Configuration.GetConnectionString("DefaultConnection"));
            
            // In development, we might want to continue even with database issues
            if (builder.Environment.IsDevelopment())
            {
                app.Logger.LogWarning("Continuing startup in development mode despite database issues.");
            }
            else
            {
                throw; // Re-throw in production
            }
        }
    }
}

app.ConfigureApiPipeline();

try
{
    Log.Information("Starting ChatBot API");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}