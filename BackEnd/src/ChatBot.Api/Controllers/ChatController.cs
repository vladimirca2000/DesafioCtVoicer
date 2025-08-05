using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using ChatBot.Application.Features.Chat.Commands.StartChatSession;
using ChatBot.Application.Features.Chat.Commands.SendMessage;
using ChatBot.Application.Features.Chat.Commands.EndChatSession; // Necessário para o novo comando
using ChatBot.Application.Features.Chat.Queries.GetActiveSessions; // Adicionado para GetActiveChatSessionQuery
using ChatBot.Application.Common.Models;

namespace ChatBot.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly IMediator _mediator;

    public ChatController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Inicia uma nova sessão de chat.
    /// </summary>
    /// <param name="command">Dados para iniciar a sessão de chat.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Detalhes da sessão de chat iniciada.</returns>
    [HttpPost("start-session")]
    [ProducesResponseType(typeof(StartChatSessionResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ChatBot.Shared.DTOs.General.ErrorResponse), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ChatBot.Shared.DTOs.General.ErrorResponse), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> StartSession([FromBody] StartChatSessionCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        
        if (!result.IsSuccess)
        {
            // Verificar se é erro de usuário não encontrado
            if (result.Errors.Any(e => e.Contains("Usuário") && e.Contains("não foi encontrado")))
            {
                return NotFound(new ChatBot.Shared.DTOs.General.ErrorResponse
                {
                    Title = "Usuário Não Encontrado",
                    Status = (int)HttpStatusCode.NotFound,
                    Detail = result.Errors.FirstOrDefault(),
                    Messages = result.Errors
                });
            }
            
            // Verificar se é erro de conteúdo da mensagem inicial inválido
            if (result.Errors.Any(e => e.Contains("Conteúdo da mensagem inicial inválido")))
            {
                return BadRequest(new ChatBot.Shared.DTOs.General.ErrorResponse
                {
                    Title = "Conteúdo da Mensagem Inicial Inválido",
                    Status = (int)HttpStatusCode.BadRequest,
                    Detail = result.Errors.FirstOrDefault(),
                    Messages = result.Errors
                });
            }
            
            // Para outros tipos de erro, retorna BadRequest
            return BadRequest(new ChatBot.Shared.DTOs.General.ErrorResponse
            {
                Title = "Falha ao Iniciar Sessão de Chat",
                Status = (int)HttpStatusCode.BadRequest,
                Detail = "Um ou mais erros ocorreram na sua requisição.",
                Messages = result.Errors
            });
        }
        
        return Ok(result.Value);
    }

    /// <summary>
    /// Envia uma mensagem para uma sessão de chat existente.
    /// </summary>
    /// <param name="command">Dados da mensagem a ser enviada.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Detalhes da mensagem enviada.</returns>
    [HttpPost("send-message")]
    [ProducesResponseType(typeof(SendMessageResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ChatBot.Shared.DTOs.General.ErrorResponse), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ChatBot.Shared.DTOs.General.ErrorResponse), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ChatBot.Shared.DTOs.General.ErrorResponse), (int)HttpStatusCode.Conflict)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        
        if (!result.IsSuccess)
        {
            // Verificar se é erro de sessão não encontrada
            if (result.Errors.Any(e => e.Contains("não foi encontrada")))
            {
                return NotFound(new ChatBot.Shared.DTOs.General.ErrorResponse
                {
                    Title = "Sessão de Chat Não Encontrada",
                    Status = (int)HttpStatusCode.NotFound,
                    Detail = result.Errors.FirstOrDefault(),
                    Messages = result.Errors
                });
            }
            
            // Verificar se é erro de usuário não encontrado
            if (result.Errors.Any(e => e.Contains("Usuário") && e.Contains("não foi encontrado")))
            {
                return NotFound(new ChatBot.Shared.DTOs.General.ErrorResponse
                {
                    Title = "Usuário Não Encontrado",
                    Status = (int)HttpStatusCode.NotFound,
                    Detail = result.Errors.FirstOrDefault(),
                    Messages = result.Errors
                });
            }
            
            // Verificar se é erro de sessão inativa
            if (result.Errors.Any(e => e.Contains("sessão de chat com status") || e.Contains("deve estar ativa")))
            {
                return Conflict(new ChatBot.Shared.DTOs.General.ErrorResponse
                {
                    Title = "Sessão de Chat Inativa",
                    Status = (int)HttpStatusCode.Conflict,
                    Detail = result.Errors.FirstOrDefault(),
                    Messages = result.Errors
                });
            }
            
            // Verificar se é erro de conteúdo inválido
            if (result.Errors.Any(e => e.Contains("Conteúdo da mensagem inválido")))
            {
                return BadRequest(new ChatBot.Shared.DTOs.General.ErrorResponse
                {
                    Title = "Conteúdo da Mensagem Inválido",
                    Status = (int)HttpStatusCode.BadRequest,
                    Detail = result.Errors.FirstOrDefault(),
                    Messages = result.Errors
                });
            }
            
            // Para outros tipos de erro, retorna BadRequest
            return BadRequest(new ChatBot.Shared.DTOs.General.ErrorResponse
            {
                Title = "Falha ao Enviar Mensagem",
                Status = (int)HttpStatusCode.BadRequest,
                Detail = "Um ou mais erros ocorreram na sua requisição.",
                Messages = result.Errors
            });
        }
        
        return Ok(result.Value);
    }

    /// <summary>
    /// Encerra uma sessão de chat existente.
    /// </summary>
    /// <param name="command">Dados para encerrar a sessão de chat.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Confirmação do encerramento da sessão.</returns>
    [HttpPost("end-session")]
    [ProducesResponseType(typeof(EndChatSessionResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ChatBot.Shared.DTOs.General.ErrorResponse), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ChatBot.Shared.DTOs.General.ErrorResponse), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ChatBot.Shared.DTOs.General.ErrorResponse), (int)HttpStatusCode.Conflict)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> EndSession([FromBody] EndChatSessionCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        
        if (!result.IsSuccess)
        {
            // Verificar se é erro de sessão não encontrada
            if (result.Errors.Any(e => e.Contains("Sessão de chat") && e.Contains("não foi encontrada")))
            {
                return NotFound(new ChatBot.Shared.DTOs.General.ErrorResponse
                {
                    Title = "Sessão de Chat Não Encontrada",
                    Status = (int)HttpStatusCode.NotFound,
                    Detail = result.Errors.FirstOrDefault(),
                    Messages = result.Errors
                });
            }
            
            // Verificar se é erro de sessão já encerrada
            if (result.Errors.Any(e => e.Contains("já está encerrada")))
            {
                return Conflict(new ChatBot.Shared.DTOs.General.ErrorResponse
                {
                    Title = "Sessão Já Encerrada",
                    Status = (int)HttpStatusCode.Conflict,
                    Detail = result.Errors.FirstOrDefault(),
                    Messages = result.Errors
                });
            }
            
            // Para outros tipos de erro, retorna BadRequest
            return BadRequest(new ChatBot.Shared.DTOs.General.ErrorResponse
            {
                Title = "Falha ao Encerrar Sessão de Chat",
                Status = (int)HttpStatusCode.BadRequest,
                Detail = "Um ou mais erros ocorreram na sua requisição.",
                Messages = result.Errors
            });
        }
        
        return Ok(result.Value);
    }

    /// <summary>
    /// Busca a sessão ativa de um usuário específico.
    /// </summary>
    /// <param name="userId">ID do usuário.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Detalhes da sessão ativa ou null se não houver.</returns>
    [HttpGet("active-session")]
    [ProducesResponseType(typeof(ActiveSessionDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ChatBot.Shared.DTOs.General.ErrorResponse), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> GetActiveSession([FromQuery] Guid userId, CancellationToken cancellationToken)
    {
        var query = new GetActiveChatSessionQuery { UserId = userId };
        var result = await _mediator.Send(query, cancellationToken);
        
        if (!result.IsSuccess)
        {
            return BadRequest(new ChatBot.Shared.DTOs.General.ErrorResponse
            {
                Title = "Falha ao Buscar Sessão Ativa",
                Status = (int)HttpStatusCode.BadRequest,
                Detail = "Um ou mais erros ocorreram na sua requisição.",
                Messages = result.Errors
            });
        }
        
        if (result.Value == null)
        {
            return NotFound();
        }
        
        return Ok(result.Value);
    }
}