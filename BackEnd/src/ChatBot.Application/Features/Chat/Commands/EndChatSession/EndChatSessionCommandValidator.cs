using FluentValidation;

namespace ChatBot.Application.Features.Chat.Commands.EndChatSession;

/// <summary>
/// Validador para o comando EndChatSessionCommand.
/// </summary>
public class EndChatSessionCommandValidator : AbstractValidator<EndChatSessionCommand>
{
    public EndChatSessionCommandValidator()
    {
        RuleFor(x => x.ChatSessionId)
            .NotEmpty().WithMessage("O ID da sessão de chat é obrigatório.");

        RuleFor(x => x.EndReason)
            .NotEmpty().When(x => !string.IsNullOrEmpty(x.EndReason)).WithMessage("O motivo do encerramento não pode ser vazio se fornecido.")
            .MaximumLength(500).WithMessage("O motivo do encerramento não pode exceder 500 caracteres.");
    }
}