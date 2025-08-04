// Filename: C:\Desenvolvimento\DocChatBoot\BackEnd\src\ChatBot.Api\Program.cs

using ChatBot.Application;
using ChatBot.Infrastructure;
using Serilog;
using ChatBot.Infrastructure.Data; // Adicione este using para ChatBotDbContext
using Microsoft.EntityFrameworkCore; // Adicione este using para o método Migrate()

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

// --- INÍCIO DA ADIÇÃO PARA MIGRAÇÕES AUTOMÁTICAS ---
// Aplica migrações do banco de dados na inicialização, apenas em ambiente de desenvolvimento.
// Baseia-se em fatos concretos e tangíveis: a necessidade de setup simplificado em dev.
if (app.Environment.IsDevelopment()) // Aprimore cada aspecto com precisão: condicional por ambiente.
{
    using (var scope = app.Services.CreateScope()) // Cria um escopo para resolver serviços, pois DbContext é scoped.
    {
        var services = scope.ServiceProvider;
        try
        {
            var dbContext = services.GetRequiredService<ChatBotDbContext>();
            app.Logger.LogInformation("Attempting to apply database migrations...");
            dbContext.Database.Migrate(); // Este método cria o DB se não existe e aplica migrações.
            app.Logger.LogInformation("Database migrations applied successfully.");

            // Opcional: Adicionar lógica para seed de dados iniciais aqui, se necessário.
            // Exemplo: await InitialDataSeeder.SeedAsync(dbContext, services);
        }
        catch (Exception ex)
        {
            app.Logger.LogError(ex, "An error occurred while migrating the database.");
            // Você pode decidir se quer relançar a exceção ou permitir que a aplicação continue
            // (se o banco de dados não for crítico para a inicialização total).
            // Para um chat, o banco é crítico, então um erro aqui indica um problema que deve parar a app.
        }
    }
}
// --- FIM DA ADIÇÃO PARA MIGRAÇÕES AUTOMÁTICAS ---


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

// SignalR Hub (garanta que está descomentado se for usar SignalR)
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