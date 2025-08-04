using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using ChatBot.Application.Features.Chat.Commands.StartChatSession;
using ChatBot.Application.Features.Chat.Commands.SendMessage;
using ChatBot.Application.Features.Chat.Commands.EndChatSession; // Necessário para o novo comando
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
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> StartSession([FromBody] StartChatSessionCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
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
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
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
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> EndSession([FromBody] EndChatSessionCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result.Value);
    }
}