using ChatBot.Domain.ValueObjects; // Necessário para MessageContent
using ChatBot.Application.Features.Bot.Commands.ProcessUserMessage;

namespace ChatBot.Application.Features.Bot.Strategies;

/// <summary>
/// Estratégia de resposta para o comando de saída ("sair").
/// </summary>
public class ExitCommandStrategy : IBotResponseStrategy
{
    public bool CanHandle(ProcessUserMessageCommand command)
    {
        return command.UserMessage.ToLowerInvariant().Trim() == "sair";
    }

    public MessageContent GenerateResponse(ProcessUserMessageCommand command)
    {
        // Aqui você pode adicionar lógica para encerrar a sessão de chat, se necessário
        return MessageContent.Create("Sessão encerrada. Até a próxima!");
    }
}