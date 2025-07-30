using FluentValidation;

namespace ChatBot.Application.Features.Chat.SendMessage;

public class SendMessageCommandValidator : AbstractValidator<SendMessageCommand>
{
    public SendMessageCommandValidator()
    {
        // A ChatSessionId é obrigatória e não pode ser um valor vazio de Guid
        RuleFor(x => x.ChatSessionId)
            .NotEmpty()
            .WithMessage("O ID da sessão de chat é obrigatório.");

        // O UserId é obrigatório e não pode ser um valor vazio de Guid
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("O ID do usuário é obrigatório.");

        // O conteúdo da mensagem não pode ser vazio e deve ter um tamanho razoável
        RuleFor(x => x.Content)
            .NotEmpty()
            .WithMessage("O conteúdo da mensagem não pode ser vazio.")
            .MaximumLength(2000) // Limite de caracteres para o conteúdo da mensagem
            .WithMessage("O conteúdo da mensagem excede o limite de 2000 caracteres.");
    }
}