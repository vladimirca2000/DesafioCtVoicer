using ChatBot.Application;
using ChatBot.Infrastructure;
using ChatBot.Api.Extensions;
using Serilog;
using Microsoft.Extensions.Configuration; // Necessário para IConfiguration

var builder = WebApplication.CreateBuilder(args);

// Serilog Configuration
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddApiServices(builder.Configuration); // Passa a configuração aqui
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline
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