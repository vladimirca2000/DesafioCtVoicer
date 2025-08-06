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
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            throw new NotFoundException("Usuário", request.UserId);
        }

        await _userRepository.DeleteAsync(request.UserId, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<DeleteUserResponse>.Success(new DeleteUserResponse
        {
            UserId = user.Id,
            IsDeleted = true,
            DeletedAt = DateTime.UtcNow
        });
    }
}