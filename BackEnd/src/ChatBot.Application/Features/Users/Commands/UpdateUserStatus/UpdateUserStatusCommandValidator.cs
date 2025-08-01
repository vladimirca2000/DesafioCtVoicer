using FluentValidation;

namespace ChatBot.Application.Features.Users.Commands.UpdateUserStatus;

/// <summary>
/// Validador para o comando UpdateUserStatusCommand.
/// </summary>
public class UpdateUserStatusCommandValidator : AbstractValidator<UpdateUserStatusCommand>
{
    public UpdateUserStatusCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("O ID do usuário é obrigatório.");
    }
}