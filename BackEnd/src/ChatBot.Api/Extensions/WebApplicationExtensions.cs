using ChatBot.Api.Hubs;
using ChatBot.Api.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace ChatBot.Api.Extensions;

public static class WebApplicationExtensions
{
    /// <summary>
    /// Configura o pipeline de requisições HTTP da camada de API.
    /// </summary>
    /// <param name="app">A aplicação web.</param>
    /// <returns>A aplicação web configurada.</returns>
    public static WebApplication ConfigureApiPipeline(this WebApplication app)
    {
        // O Middleware de tratamento de exceções é crucial e deve ser um dos primeiros.
        app.UseMiddleware<ExceptionHandlingMiddleware>();

        // Configure o pipeline de requisições HTTP
        // Em produção, UseSwagger e UseSwaggerUI NÃO devem ser habilitados.
        // O if (app.Environment.IsDevelopment()) já cuida disso.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // HTTPS Enforcement: Redireciona todas as requisições HTTP para HTTPS. ESSENCIAL para produção.
        app.UseHttpsRedirection();

        // CORS: Deve vir antes de UseAuthorization/MapControllers.
        app.UseCors("SpecificCorsPolicy"); // Aplica a política de CORS renomeada

        // Autenticação/Autorização: Deve vir antes de MapControllers.
        // Aqui você integraria sua lógica de autenticação (ex: JWT Bearer Token, Identity).
        app.UseAuthorization();

        app.MapControllers();

        // SignalR Hub
        app.MapHub<ChatHub>("/chathub");

        return app;
    }
}