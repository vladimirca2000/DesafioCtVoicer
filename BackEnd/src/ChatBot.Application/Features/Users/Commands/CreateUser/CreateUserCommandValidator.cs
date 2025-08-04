using FluentValidation;

namespace ChatBot.Application.Features.Users.Commands.CreateUser;

/// <summary>
/// Validador para o comando CreateUserCommand.
/// A validação de formato de e-mail é feita no Value Object Email.
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
            // A validação de formato de e-mail mais robusta está no construtor do Value Object Email.
            // Aqui, podemos manter uma validação básica ou remover se o VO for suficiente.
            // Manter `EmailAddress()` para feedback imediato na camada de entrada.
            .EmailAddress().WithMessage("O formato do e-mail é inválido.")
            .MaximumLength(255).WithMessage("O e-mail não pode exceder 255 caracteres.");
    }
}