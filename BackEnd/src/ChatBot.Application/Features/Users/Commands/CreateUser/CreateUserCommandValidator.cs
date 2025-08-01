using FluentValidation;

namespace ChatBot.Application.Features.Users.Commands.CreateUser;

/// <summary>
/// Validador para o comando CreateUserCommand.
/// </summary>
public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O nome do usuário é obrigatório.")
            .MaximumLength(100).WithMessage("O nome do usuário não pode exceder 100 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O e-mail é obrigatório.")
            .EmailAddress().WithMessage("O formato do e-mail é inválido.")
            .MaximumLength(255).WithMessage("O e-mail não pode exceder 255 caracteres.");
    }
}
