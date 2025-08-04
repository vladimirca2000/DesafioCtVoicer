using FluentValidation;

namespace ChatBot.Application.Features.Chat.Commands.StartChatSession;

public class StartChatSessionCommandValidator : AbstractValidator<StartChatSessionCommand>
{
    public StartChatSessionCommandValidator()
    {
        // Se UserId for nulo, UserName é obrigatório
        RuleFor(x => x.UserName)
            .NotEmpty()
            .When(x => !x.UserId.HasValue)
            .WithMessage("O nome de usuário é obrigatório se nenhum ID de usuário for fornecido.");

        // Mensagem inicial não pode ser vazia
        // A validação completa de tamanho e conteúdo é feita no Value Object MessageContent.
        RuleFor(x => x.InitialMessageContent)
            .NotEmpty()
            .WithMessage("A mensagem inicial não pode ser vazia.");
    }
}