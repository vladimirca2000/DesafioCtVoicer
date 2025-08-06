using ChatBot.Application;
using ChatBot.Infrastructure;
using Serilog;
using ChatBot.Infrastructure.Data; // Adicione este using para ChatBotDbContext
using Microsoft.EntityFrameworkCore; // Adicione este using para o mtodo Migrate()
using ChatBot.Api.Extensions; // Adicione este using para os mtodos de extenso da API
using ChatBot.Infrastructure.Services; // Adicionando using para o serviço de background

var builder = WebApplication.CreateBuilder(args);

// Serilog Configuration
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// REMOVIDO: O registro de CORS e SignalR agora ser feito via AddApiServices para centralizar.
// SignalR (aqui ficar apenas o MapHub, o registro do servio ser via AddApiServices)
builder.Services.AddSignalR();

// Adiciona o serviço de limpeza de sessões inativas
builder.Services.AddHostedService<InactiveSessionCleanupService>();

// Chamada crucial para registrar os servios especficos da camada de API, incluindo SignalR e ICurrentUserService.
// Isso inclui o CORS configurado em ServiceCollectionExtensions.
builder.Services.AddApiServices(builder.Configuration); // <--- LINHA ADICIONADA/ALTERADA AQUI!

// Application & Infrastructure (mantidas)
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Adiciona o middleware global de tratamento de exceções
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

//// Configure the HTTP request pipeline
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();
//app.UseAuthorization();
//app.MapControllers();

//// SignalR Hub (garanta que est descomentado se for usar SignalR)
//app.MapHub<ChatBot.Api.Hubs.ChatHub>("/chathub"); // Certifique-se de usar o namespace completo aqui

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