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
       
        app.UseMiddleware<ExceptionHandlingMiddleware>();

        
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        
        app.UseHttpsRedirection();

        
        app.UseCors("SpecificCorsPolicy"); 

        
        app.UseAuthorization();

        
        app.MapHealthChecks("/health");

        app.MapControllers();

        
        app.MapHub<ChatHub>("/chathub");

        return app;
    }
}