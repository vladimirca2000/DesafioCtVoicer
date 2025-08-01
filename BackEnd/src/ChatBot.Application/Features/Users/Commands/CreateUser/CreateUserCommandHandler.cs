using MediatR;
using ChatBot.Application.Common.Models;
using ChatBot.Application.Common.Interfaces;
using ChatBot.Domain.Repositories;
using ChatBot.Domain.Entities;
using ChatBot.Application.Common.Exceptions;

namespace ChatBot.Application.Features.Users.Commands.CreateUser;

/// <summary>
/// Manipulador para o comando CreateUserCommand.
/// </summary>
public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<CreateUserResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CreateUserResponse>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // 1. Verificar se o e-mail já está em uso (excluindo usuários logicamente deletados se a regra de negócio permitir)
        // O GetUserByEmailAsync já deve ignorar usuários deletados devido ao Global Query Filter no DbContext
        var existingUser = await _userRepository.GetUserByEmailAsync(request.Email, cancellationToken);
        if (existingUser != null)
        {
            return Result<CreateUserResponse>.Failure($"Já existe um usuário com o e-mail '{request.Email}'.");
        }

        // 2. Criar a nova entidade de usuário
        var newUser = new User
        {
            Name = request.Name,
            Email = request.Email,
            IsActive = request.IsActive,
            CreatedBy = "System" // No futuro, obter do contexto do usuário logado
        };

        // 3. Adicionar o usuário ao repositório
        await _userRepository.AddAsync(newUser, cancellationToken);

        // 4. Salvar as mudanças na unidade de trabalho
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 5. Retornar a resposta de sucesso
        return Result<CreateUserResponse>.Success(new CreateUserResponse
        {
            UserId = newUser.Id,
            Name = newUser.Name,
            Email = newUser.Email,
            CreatedAt = newUser.CreatedAt
        });
    }
}