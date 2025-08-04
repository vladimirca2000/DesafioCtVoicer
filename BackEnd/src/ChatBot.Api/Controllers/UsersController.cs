using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using ChatBot.Application.Features.Users.Commands.CreateUser;
using ChatBot.Application.Features.Users.Commands.UpdateUserStatus;
using ChatBot.Application.Features.Users.Commands.DeleteUser;
using ChatBot.Application.Features.Users.Queries.GetUserById;
using ChatBot.Application.Features.Users.Queries.GetUserSessions;
using ChatBot.Application.Common.Models; // Necessário para Result<T> nos retornos de MediatR.Send()
using System.Collections.Generic; // Necessário para IEnumerable

namespace ChatBot.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Cria um novo usuário no sistema.
    /// </summary>
    /// <param name="command">Dados para criação do usuário.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Detalhes do usuário criado.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(CreateUserResponse), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)] // Para validação ou BusinessRuleException
    [ProducesResponseType((int)HttpStatusCode.Conflict)] // Se o email já existe (poderia ser tratado como BusinessRule)
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)] // Para erros genéricos
    public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return StatusCode((int)HttpStatusCode.Created, result.Value);
    }

    /// <summary>
    /// Atualiza o status (ativo/inativo) de um usuário existente.
    /// </summary>
    /// <param name="userId">ID do usuário a ser atualizado.</param>
    /// <param name="command">Dados para atualização do status.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Detalhes do status atualizado do usuário.</returns>
    [HttpPut("{userId}/status")]
    [ProducesResponseType(typeof(UpdateUserStatusResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> UpdateUserStatus(Guid userId, [FromBody] UpdateUserStatusCommand command, CancellationToken cancellationToken)
    {
        // Garante que o ID da rota corresponda ao ID do corpo, se o comando o incluir
        if (userId != command.UserId)
        {
            return BadRequest(new { errors = new[] { "O ID do usuário na rota não corresponde ao ID no corpo da requisição." } });
        }

        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result.Value);
    }

    /// <summary>
    /// Realiza o soft delete de um usuário pelo seu ID.
    /// </summary>
    /// <param name="userId">ID do usuário a ser deletado.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Confirmação da exclusão.</returns>
    [HttpDelete("{userId}")]
    [ProducesResponseType(typeof(DeleteUserResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> DeleteUser(Guid userId, CancellationToken cancellationToken)
    {
        var command = new DeleteUserCommand { UserId = userId };
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result.Value);
    }

    /// <summary>
    /// Obtém os detalhes de um usuário pelo seu ID.
    /// </summary>
    /// <param name="userId">ID do usuário.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Detalhes do usuário.</returns>
    [HttpGet("{userId}")]
    [ProducesResponseType(typeof(UserDetailDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> GetUserById(Guid userId, CancellationToken cancellationToken)
    {
        var query = new GetUserByIdQuery { UserId = userId };
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result.Value);
    }

    /// <summary>
    /// Obtém todas as sessões de chat de um usuário específico.
    /// </summary>
    /// <param name="userId">ID do usuário.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Lista de sessões de chat do usuário.</returns>
    [HttpGet("{userId}/sessions")]
    [ProducesResponseType(typeof(IEnumerable<UserSessionDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> GetUserSessions(Guid userId, CancellationToken cancellationToken)
    {
        var query = new GetUserSessionsQuery { UserId = userId };
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result.Value);
    }
}