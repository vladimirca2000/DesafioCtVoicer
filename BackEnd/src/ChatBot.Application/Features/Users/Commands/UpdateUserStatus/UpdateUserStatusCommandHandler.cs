using MediatR;
using ChatBot.Application.Common.Models;
using ChatBot.Application.Common.Interfaces;
using ChatBot.Domain.Repositories;
using ChatBot.Application.Common.Exceptions;

namespace ChatBot.Application.Features.Users.Commands.UpdateUserStatus;

/// <summary>
/// Manipulador para o comando UpdateUserStatusCommand.
/// </summary>
public class UpdateUserStatusCommandHandler : IRequestHandler<UpdateUserStatusCommand, Result<UpdateUserStatusResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserStatusCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<UpdateUserStatusResponse>> Handle(UpdateUserStatusCommand request, CancellationToken cancellationToken)
    {
        // 1. Obter o usuário existente
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            return Result<UpdateUserStatusResponse>.Failure($"Usuário com ID '{request.UserId}' não encontrado.");
        }

        // 2. Atualizar o status
        user.IsActive = request.IsActive;
        user.UpdatedBy = "System"; // No futuro, obter do contexto do usuário logado

        // 3. Atualizar o usuário no repositório
        await _userRepository.UpdateAsync(user, cancellationToken);

        // 4. Salvar as mudanças
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 5. Retornar a resposta de sucesso
        return Result<UpdateUserStatusResponse>.Success(new UpdateUserStatusResponse
        {
            UserId = user.Id,
            IsActive = user.IsActive,
            UpdatedAt = user.UpdatedAt ?? DateTime.UtcNow // Garantir que UpdatedAt tenha um valor
        });
    }
}