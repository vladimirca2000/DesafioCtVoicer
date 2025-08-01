using FluentValidation;

namespace ChatBot.Application.Features.Users.Commands.DeleteUser;

/// <summary>
/// Validador para o comando DeleteUserCommand.
/// </summary>
public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("O ID do usuário é obrigatório.");
    }
}