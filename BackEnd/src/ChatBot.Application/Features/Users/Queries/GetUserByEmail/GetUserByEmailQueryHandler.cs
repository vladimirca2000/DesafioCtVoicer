using MediatR;
using ChatBot.Application.Common.Models;
using ChatBot.Domain.Repositories;
using ChatBot.Application.Common.Exceptions;
using ChatBot.Application.Features.Users.Queries.GetUserById; // Para usar UserDetailDto
using ChatBot.Domain.ValueObjects; // Para usar Email Value Object

namespace ChatBot.Application.Features.Users.Queries.GetUserByEmail;

/// <summary>
/// Manipulador para a query GetUserByEmailQuery.
/// </summary>
public class GetUserByEmailQueryHandler : IRequestHandler<GetUserByEmailQuery, Result<UserDetailDto>>
{
    private readonly IUserRepository _userRepository;

    public GetUserByEmailQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<UserDetailDto>> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
    {
        // 1. Criar o Value Object Email a partir da string de entrada.
        // A validao do formato j ocorre no construtor do Email VO.
        var userEmail = Email.Create(request.Email);

        // 2. Obter o usurio pelo e-mail
        var user = await _userRepository.GetUserByEmailAsync(userEmail, cancellationToken);
        if (user == null)
        {
            throw new NotFoundException($"Usurio com e-mail '{request.Email}' no encontrado.");
        }

        // 3. Mapear para o DTO (convertendo o VO Email de volta para string para o DTO de sada)
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

        // 4. Retornar o DTO de sucesso
        return Result<UserDetailDto>.Success(userDetailDto);
    }
}