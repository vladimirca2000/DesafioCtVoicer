using MediatR; 
using Microsoft.AspNetCore.Mvc;
using System.Net; 
using ChatBot.Application.Features.Bot.Commands.ProcessUserMessage; 
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
        return Ok("Olá sou V]Bot de F1");
    }

    /// <summary>
    /// Processa uma mensagem de usuário e retorna a resposta do bot.
    /// Observação: Este endpoint pode ser desnecessário se o BotAutoResponseEventHandler estiver funcionando.
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
        if (!result.IsSuccess)
        {
            return BadRequest(new ChatBot.Shared.DTOs.General.ErrorResponse
            {
                Title = "Falha ao Processar Mensagem",
                Status = (int)HttpStatusCode.BadRequest,
                Detail = "Um ou mais erros ocorreram ao processar a mensagem.",
                Messages = result.Errors
            });
        }
        return Ok(result.Value);
    }
}