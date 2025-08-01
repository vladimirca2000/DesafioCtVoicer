using MediatR;
using ChatBot.Application.Common.Models;
using ChatBot.Application.Common.Interfaces;
using ChatBot.Domain.Repositories;
using ChatBot.Application.Common.Exceptions;

namespace ChatBot.Application.Features.Users.Queries.GetUserById;

/// <summary>
/// Manipulador para a query GetUserByIdQuery.
/// </summary>
public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<UserDetailDto>>
{
    private readonly IUserRepository _userRepository;

    public GetUserByIdQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<UserDetailDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        // 1. Obter o usuário pelo ID
        // O GetByIdAsync já considera o Global Query Filter, então não trará usuários deletados.
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            return Result<UserDetailDto>.Failure($"Usuário com ID '{request.UserId}' não encontrado ou está inativo/deletado.");
        }

        // 2. Mapear para o DTO
        var userDetailDto = new UserDetailDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            IsDeleted = user.IsDeleted
        };

        // 3. Retornar o DTO de sucesso
        return Result<UserDetailDto>.Success(userDetailDto);
    }
}