using MediatR;
using ChatBot.Application.Common.Models;
using ChatBot.Application.Common.Interfaces;
using ChatBot.Domain.Repositories;
using ChatBot.Application.Common.Exceptions;

namespace ChatBot.Application.Features.Users.Commands.DeleteUser;

/// <summary>
/// Manipulador para o comando DeleteUserCommand (soft delete).
/// </summary>
public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Result<DeleteUserResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<DeleteUserResponse>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        // 1. Verificar se o usuário existe e não está já logicamente deletado
        // Precisamos ignorar o filtro global para verificar usuários já deletados se quisermos restaurar ou ter certeza.
        // Mas para um delete comum, GetByIdAsync já exclui deletados, então se retornar null, é porque não existe ou já está deletado.
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            // Poderíamos buscar com IgnoreQueryFilters para dar uma mensagem mais específica
            var userCheck = await _userRepository.GetAllAsync(cancellationToken); // Exemplo, buscar todos e filtrar
            var existingDeletedUser = userCheck.FirstOrDefault(u => u.Id == request.UserId && u.IsDeleted);

            if (existingDeletedUser != null)
            {
                return Result<DeleteUserResponse>.Failure($"Usuário com ID '{request.UserId}' já está logicamente deletado.");
            }
            return Result<DeleteUserResponse>.Failure($"Usuário com ID '{request.UserId}' não encontrado.");
        }

        // 2. Realizar o soft delete
        await _userRepository.DeleteAsync(request.UserId, cancellationToken);

        // 3. Salvar as mudanças (o interceptor cuidará de marcar IsDeleted e DeletedAt)
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 4. Retornar a resposta de sucesso
        return Result<DeleteUserResponse>.Success(new DeleteUserResponse
        {
            UserId = user.Id,
            IsDeleted = true, // Assumimos que foi deletado com sucesso
            DeletedAt = DateTime.UtcNow // O interceptor preenche, mas para resposta podemos usar agora
        });
    }
}