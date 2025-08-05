// Conteúdo COMPLETO para ChatBot.Api/Controllers/UsersController.cs

using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using ChatBot.Application.Features.Users.Commands.CreateUser;
using ChatBot.Application.Features.Users.Commands.UpdateUserStatus;
using ChatBot.Application.Features.Users.Commands.DeleteUser;
using ChatBot.Application.Features.Users.Queries.GetUserById;
using ChatBot.Application.Features.Users.Queries.GetUserSessions;
using ChatBot.Application.Common.Models;
using System.Collections.Generic;
using ChatBot.Application.Features.Users.Queries.GetUserByEmail;
using ChatBot.Shared.DTOs.General; // Adicionado para usar ErrorResponse

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
    /// Cria um novo usurio no sistema.
    /// </summary>
    /// <param name="command">Dados para criao do usurio.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Detalhes do usurio criado.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(CreateUserResponse), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)] // Alterado para refletir retorno de ErrorResponse
    [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.Conflict)] // Adicionado para refletir conflito de regras de negócio
    [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        if (!result.IsSuccess)
        {
            // Verifica se o erro é de usuário duplicado para retornar 409 Conflict
            if (result.Errors.Any(e => e.Contains("Já existe um usuário com o e-mail")))
            {
                return Conflict(new ErrorResponse
                {
                    Title = "Usuário Já Existe",
                    Status = (int)HttpStatusCode.Conflict,
                    Detail = result.Errors.FirstOrDefault(),
                    Messages = result.Errors
                });
            }
            
            // Para outros tipos de erro, retorna BadRequest
            return BadRequest(new ErrorResponse
            {
                Title = "Falha ao criar usuário",
                Status = (int)HttpStatusCode.BadRequest,
                Detail = "Um ou mais erros ocorreram na sua requisição.",
                Messages = result.Errors
            });
        }
        return StatusCode((int)HttpStatusCode.Created, result.Value);
    }

    /// <summary>
    /// Atualiza o status (ativo/inativo) de um usurio existente.
    /// </summary>
    /// <param name="userId">ID do usurio a ser atualizado.</param>
    /// <param name="command">Dados para atualizao do status.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Detalhes do status atualizado do usurio.</returns>
    [HttpPut("{userId}/status")]
    [ProducesResponseType(typeof(UpdateUserStatusResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> UpdateUserStatus(Guid userId, [FromBody] UpdateUserStatusCommand command, CancellationToken cancellationToken)
    {
        if (userId != command.UserId)
        {
            return BadRequest(new ErrorResponse
            {
                Title = "Requisição Inválida",
                Status = (int)HttpStatusCode.BadRequest,
                Detail = "O ID do usuário na rota não corresponde ao ID no corpo da requisição.",
                Messages = new List<string> { "O ID do usuário na rota não corresponde ao ID no corpo da requisição." }
            });
        }
        var result = await _mediator.Send(command, cancellationToken);
        if (!result.IsSuccess)
        {
            // Similar ao CreateUser, o middleware ainda pegará as NotFoundException e BusinessRuleException
            // Este bloco seria mais útil se o handler retornasse Result.Failure para NotFound/BusinessRuleException
            // sem lançar uma exceção.
            if (result.Errors.Any(e => e.Contains("não encontrado"))) // Exemplo de heurística para NotFound
            {
                return NotFound(new ErrorResponse
                {
                    Title = "Usuário Não Encontrado",
                    Status = (int)HttpStatusCode.NotFound,
                    Detail = result.Errors.FirstOrDefault(),
                    Messages = result.Errors
                });
            }
            return BadRequest(new ErrorResponse
            {
                Title = "Falha ao atualizar status",
                Status = (int)HttpStatusCode.BadRequest,
                Detail = "Um ou mais erros ocorreram na sua requisição.",
                Messages = result.Errors
            });
        }
        return Ok(result.Value);
    }

    /// <summary>
    /// Realiza o soft delete de um usurio pelo seu ID.
    /// </summary>
    /// <param name="userId">ID do usurio a ser deletado.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Confirmao da excluso.</returns>
    [HttpDelete("{userId}")]
    [ProducesResponseType(typeof(DeleteUserResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> DeleteUser(Guid userId, CancellationToken cancellationToken)
    {
        var command = new DeleteUserCommand { UserId = userId };
        var result = await _mediator.Send(command, cancellationToken);
        if (!result.IsSuccess)
        {
            if (result.Errors.Any(e => e.Contains("não encontrado")))
            {
                return NotFound(new ErrorResponse { Title = "Usuário Não Encontrado", Status = (int)HttpStatusCode.NotFound, Detail = result.Errors.FirstOrDefault(), Messages = result.Errors });
            }
            return BadRequest(new ErrorResponse { Title = "Falha ao deletar usuário", Status = (int)HttpStatusCode.BadRequest, Detail = "Um ou mais erros ocorreram.", Messages = result.Errors });
        }
        return Ok(result.Value);
    }

    /// <summary>
    /// Obtm os detalhes de um usurio pelo seu ID.
    /// </summary>
    /// <param name="userId">ID do usurio.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Detalhes do usurio.</returns>
    [HttpGet("{userId}")]
    [ProducesResponseType(typeof(UserDetailDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> GetUserById(Guid userId, CancellationToken cancellationToken)
    {
        var query = new GetUserByIdQuery { UserId = userId };
        var result = await _mediator.Send(query, cancellationToken);
        if (!result.IsSuccess)
        {
            // O handler GetUserByIdQueryHandler lança NotFoundException, que é pego pelo middleware.
            // Para explicitá-lo aqui, você precisaria que o handler retornasse Result.Failure em vez de lançar.
            // Se o handler fosse modificado para retornar Result.Failure(new NotFoundException(...).Message),
            // então você poderia usar:
            // return NotFound(new ErrorResponse { Title = "Usuário Não Encontrado", Status = (int)HttpStatusCode.NotFound, Detail = result.Errors.FirstOrDefault(), Messages = result.Errors });
            // Por enquanto, o middleware ainda é o responsável final pelo 404.
            // Manteremos a lógica para fins didáticos de como seria se o handler retornasse a falha.
            return NotFound(new ErrorResponse
            {
                Title = "Usuário Não Encontrado",
                Status = (int)HttpStatusCode.NotFound,
                Detail = "O usuário solicitado não foi encontrado.",
                Messages = new List<string> { "O usuário solicitado não foi encontrado." }
            });
        }
        return Ok(result.Value);
    }

    /// <summary>
    /// Obtm os detalhes de um usurio pelo seu endereo de e-mail.
    /// </summary>
    /// <param name="email">Endereo de e-mail do usurio.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Detalhes do usurio.</returns>
    [HttpGet("by-email")]
    [ProducesResponseType(typeof(UserDetailDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> GetUserByEmail([FromQuery] string email, CancellationToken cancellationToken)
    {
        var query = new GetUserByEmailQuery { Email = email };
        var result = await _mediator.Send(query, cancellationToken);
        if (!result.IsSuccess)
        {
            // O handler GetUserByEmailQueryHandler lança NotFoundException, que é pego pelo middleware.
            // O ValidationBehavior pega a ValidationException.
            // Se o handler retornasse Result.Failure para NotFound/Validation, a lógica abaixo seria ativada.
            if (result.Errors.Any(e => e.Contains("não encontrado")))
            {
                return NotFound(new ErrorResponse { Title = "Usuário Não Encontrado", Status = (int)HttpStatusCode.NotFound, Detail = result.Errors.FirstOrDefault(), Messages = result.Errors });
            }
            return BadRequest(new ErrorResponse { Title = "E-mail inválido ou falha na busca", Status = (int)HttpStatusCode.BadRequest, Detail = result.Errors.FirstOrDefault(), Messages = result.Errors });
        }
        return Ok(result.Value);
    }

    /// <summary>
    /// Obtm todas as sesses de chat de um usurio especfico.
    /// </summary>
    /// <param name="userId">ID do usurio.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Lista de sesses de chat do usurio.</returns>
    [HttpGet("{userId}/sessions")]
    [ProducesResponseType(typeof(IEnumerable<UserSessionDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> GetUserSessions(Guid userId, CancellationToken cancellationToken)
    {
        var query = new GetUserSessionsQuery { UserId = userId };
        var result = await _mediator.Send(query, cancellationToken);
        if (!result.IsSuccess)
        {
            if (result.Errors.Any(e => e.Contains("não encontrado")))
            {
                return NotFound(new ErrorResponse { Title = "Usuário Não Encontrado", Status = (int)HttpStatusCode.NotFound, Detail = result.Errors.FirstOrDefault(), Messages = result.Errors });
            }
            return BadRequest(new ErrorResponse { Title = "Falha ao obter sessões do usuário", Status = (int)HttpStatusCode.BadRequest, Detail = result.Errors.FirstOrDefault(), Messages = result.Errors });
        }
        return Ok(result.Value);
    }
}