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
{
    using (var scope = app.Services.CreateScope()) 
    {
        var services = scope.ServiceProvider;
        try
        {
            var dbContext = services.GetRequiredService<ChatBotDbContext>();
            app.Logger.LogInformation("Attempting to apply database migrations...");
            dbContext.Database.Migrate(); 
            app.Logger.LogInformation("Database migrations applied successfully.");
        }
        catch (Exception ex)
        {
            app.Logger.LogError(ex, "An error occurred while migrating the database.");
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