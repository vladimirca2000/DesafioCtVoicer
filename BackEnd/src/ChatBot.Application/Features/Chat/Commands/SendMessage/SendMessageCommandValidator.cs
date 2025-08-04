using FluentValidation;

namespace ChatBot.Application.Features.Chat.Commands.SendMessage;

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

        // O conteúdo da mensagem não pode ser vazio e deve ter um tamanho razoável.
        // A validação completa de tamanho e conteúdo é feita no Value Object MessageContent.
        RuleFor(x => x.Content)
            .NotEmpty()
            .WithMessage("O conteúdo da mensagem não pode ser vazio.")
            .MaximumLength(2000) // Limite de caracteres para o conteúdo da mensagem, em linha com o VO
            .WithMessage("O conteúdo da mensagem excede o limite de 2000 caracteres.");
    }
}