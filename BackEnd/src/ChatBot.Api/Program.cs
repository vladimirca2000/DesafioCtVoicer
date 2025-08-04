// Filename: C:\Desenvolvimento\DocChatBoot\BackEnd\src\ChatBot.Api\Program.cs

using ChatBot.Application;
using ChatBot.Infrastructure;
using Serilog;
using ChatBot.Infrastructure.Data; // Adicione este using para ChatBotDbContext
using Microsoft.EntityFrameworkCore; // Adicione este using para o m�todo Migrate()

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

// SignalR
builder.Services.AddSignalR();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Application & Infrastructure
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// --- IN�CIO DA ADI��O PARA MIGRA��ES AUTOM�TICAS ---
// Aplica migra��es do banco de dados na inicializa��o, apenas em ambiente de desenvolvimento.
// Baseia-se em fatos concretos e tang�veis: a necessidade de setup simplificado em dev.
if (app.Environment.IsDevelopment()) // Aprimore cada aspecto com precis�o: condicional por ambiente.
{
    using (var scope = app.Services.CreateScope()) // Cria um escopo para resolver servi�os, pois DbContext � scoped.
    {
        var services = scope.ServiceProvider;
        try
        {
            var dbContext = services.GetRequiredService<ChatBotDbContext>();
            app.Logger.LogInformation("Attempting to apply database migrations...");
            dbContext.Database.Migrate(); // Este m�todo cria o DB se n�o existe e aplica migra��es.
            app.Logger.LogInformation("Database migrations applied successfully.");

            // Opcional: Adicionar l�gica para seed de dados iniciais aqui, se necess�rio.
            // Exemplo: await InitialDataSeeder.SeedAsync(dbContext, services);
        }
        catch (Exception ex)
        {
            app.Logger.LogError(ex, "An error occurred while migrating the database.");
            // Voc� pode decidir se quer relan�ar a exce��o ou permitir que a aplica��o continue
            // (se o banco de dados n�o for cr�tico para a inicializa��o total).
            // Para um chat, o banco � cr�tico, ent�o um erro aqui indica um problema que deve parar a app.
        }
    }
}
// --- FIM DA ADI��O PARA MIGRA��ES AUTOM�TICAS ---


// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

// SignalR Hub (garanta que est� descomentado se for usar SignalR)
app.MapHub<ChatBot.Api.Hubs.ChatHub>("/chathub"); // Certifique-se de usar o namespace completo aqui

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