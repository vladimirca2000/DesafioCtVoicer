// C:\Desenvolvimento\DocChatBoot\BackEnd\src\ChatBot.Api\Middleware\ExceptionHandlingMiddleware.cs

using System.Net;
using System.Text.Json;
using ChatBot.Application.Common.Exceptions;
using ChatBot.Shared.DTOs.General;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ChatBot.Api.Middleware;

/// <summary>
/// Middleware customizado para centralizar o tratamento de exceções na API.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Tenta executar a próxima parte do pipeline da requisição
            await _next(context);
        }
        catch (Exception ex)
        {
            // Captura qualquer exceção não tratada e a loga
            _logger.LogError(ex, "Ocorreu uma exceção não tratada durante a requisição: {Message}", ex.Message);

            // Chama o método para lidar com a exceção e gerar a resposta HTTP
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // Define o tipo de conteúdo da resposta como JSON
        context.Response.ContentType = "application/json";

        // Inicializa o status code e o objeto de resposta de erro com valores padrão para erro interno do servidor (500)
        var statusCode = HttpStatusCode.InternalServerError;
        var errorResponse = new ErrorResponse
        {
            Title = "Ocorreu um erro interno no servidor.",
            Status = (int)statusCode,
            Detail = "Por favor, tente novamente mais tarde. Se o problema persistir, contate o suporte.",
            Messages = new List<string> { "Por favor, tente novamente mais tarde. Se o problema persistir, contate o suporte." }
        };

        // Usa um switch para tratar diferentes tipos de exceção
        switch (exception)
        {
            case ValidationException validationException:
                // Erros de validação (FluentValidation)
                statusCode = HttpStatusCode.BadRequest; // 400 Bad Request
                errorResponse.Title = "Erro de Validação";
                errorResponse.Detail = "Um ou mais erros de validação ocorreram na sua requisição.";
                errorResponse.Errors = validationException.Errors; // Dicionário de erros por campo
                errorResponse.Messages = validationException.Errors.SelectMany(e => e.Value).ToList(); // Lista plana de todas as mensagens de erro
                break;

            case NotFoundException notFoundException: // <--- Seu código já trata essa exceção e informa 404!
                // Recurso não encontrado
                statusCode = HttpStatusCode.NotFound; // 404 Not Found
                errorResponse.Title = "Recurso Não Encontrado";
                errorResponse.Detail = notFoundException.Message;
                errorResponse.Messages = new List<string> { notFoundException.Message };
                break;

            case BusinessRuleException businessRuleException:
                // Erros de regra de negócio
                statusCode = HttpStatusCode.BadRequest; // 400 Bad Request (poderia ser 422 Unprocessable Entity para ser mais específico)
                errorResponse.Title = "Violação de Regra de Negócio";
                errorResponse.Detail = businessRuleException.Message;
                errorResponse.Messages = new List<string> { businessRuleException.Message };
                break;

            // Default: para qualquer outra exceção não tratada, mantém 500 Internal Server Error
            default:
                break;
        }

        // Define o status code da resposta HTTP
        context.Response.StatusCode = (int)statusCode;

        // Serializa o objeto de resposta de erro para JSON
        var jsonResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        // Escreve a resposta JSON no corpo da resposta HTTP
        await context.Response.WriteAsync(jsonResponse);
    }
}