using MediatR;
using ChatBot.Application.Common.Models;
using ChatBot.Application.Common.Interfaces;
using ChatBot.Domain.Repositories;
using ChatBot.Application.Common.Exceptions;
using ChatBot.Domain.ValueObjects; // Necessário para Email (embora não criado aqui, a entidade o usa)

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
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            throw new NotFoundException("Usuário", request.UserId);
        }

        // 2. Mapear para o DTO (convertendo o VO Email de volta para string para o DTO de saída)
        var userDetailDto = new UserDetailDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email.Value, // Acessa o valor string do Value Object Email
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            IsDeleted = user.IsDeleted
        };

        // 3. Retornar o DTO de sucesso
        return Result<UserDetailDto>.Success(userDetailDto);
    }
}