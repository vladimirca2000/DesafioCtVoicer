using MediatR; // Necessário para IMediator
using Microsoft.AspNetCore.Mvc;
using System.Net; // Necessário para HttpStatusCode
using ChatBot.Application.Features.Bot.Commands.ProcessUserMessage; // O comando

namespace ChatBot.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BotController : ControllerBase
{
    private readonly IMediator _mediator;

    public BotController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Endpoint para testar o BotController.
    /// </summary>
    [HttpGet]
    public IActionResult Index()
    {
        return Ok("Hello from BotController");
    }

    /// <summary>
    /// Processa uma mensagem de usuário e retorna a resposta do bot.
    /// </summary>
    /// <param name="command">Comando contendo a mensagem do usuário e detalhes da sessão.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A resposta gerada pelo bot.</returns>
    [HttpPost("process-message")]
    [ProducesResponseType(typeof(ProcessUserMessageResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)] // Para validação, BusinessRuleException
    [ProducesResponseType((int)HttpStatusCode.NotFound)]   // Para NotFoundException
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)] // Para erros genéricos
    public async Task<IActionResult> ProcessUserMessage([FromBody] ProcessUserMessageCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result.Value); // O middleware de exceções cuidará dos casos de falha.
    }
}