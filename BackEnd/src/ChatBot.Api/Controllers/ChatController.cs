using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net; // Para HttpStatusCode
using System.Linq;
using ChatBot.Application.Features.Chat.Commands.StartChatSession;
using ChatBot.Application.Features.Chat.Commands.SendMessage;
using ChatBot.Application.Features; // Para o .Any()

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
    public async Task<IActionResult> StartSession([FromBody] StartChatSessionCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        // Retorna BadRequest com a lista de erros ou o erro principal
        return BadRequest(new { errors = result.Errors.Any() ? result.Errors : new List<string> { result.Error! } });
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
    public async Task<IActionResult> SendMessage([FromBody] SendMessageCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        // Retorna BadRequest com a lista de erros ou o erro principal
        return BadRequest(new { errors = result.Errors.Any() ? result.Errors : new List<string> { result.Error! } });
    }
}