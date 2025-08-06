using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;
using ChatBot.Application.Common.Exceptions;
using ChatBot.Shared.DTOs.General;

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
            await _next(context);
        }
        catch (ValidationException ex)
        {
            await WriteError(context, HttpStatusCode.BadRequest, "Falha de Validação", ex.Message, ex.Errors?.SelectMany(pair => pair.Value).ToList() ?? new List<string> { ex.Message });
        }
        catch (NotFoundException ex)
        {
            await WriteError(context, HttpStatusCode.NotFound, "Recurso Não Encontrado", ex.Message);
        }
        catch (UnauthorizedException ex)
        {
            await WriteError(context, HttpStatusCode.Unauthorized, "Não Autorizado", ex.Message);
        }
        catch (ForbiddenException ex)
        {
            await WriteError(context, HttpStatusCode.Forbidden, "Acesso Proibido", ex.Message);
        }
        catch (ConflictException ex)
        {
            await WriteError(context, HttpStatusCode.Conflict, "Conflito de Dados", ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado no pipeline");
            await WriteError(context, HttpStatusCode.InternalServerError, "Erro Interno do Servidor", ex.Message, new List<string> { "Ocorreu um erro inesperado. Tente novamente mais tarde." });
        }
    }

    private async Task WriteError(HttpContext context, HttpStatusCode status, string title, string detail, List<string>? messages = null)
    {
        _logger.LogWarning($"Retornando erro {status}: {title} - {detail}");
        context.Response.StatusCode = (int)status;
        context.Response.ContentType = "application/json";
        var errorResponse = new ErrorResponse
        {
            Title = title,
            Status = (int)status,
            Detail = detail,
            Messages = messages ?? new List<string> { detail }
        };
        await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
    }
}