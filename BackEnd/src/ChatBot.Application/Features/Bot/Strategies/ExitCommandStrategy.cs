// Conteúdo CORRETO e COMPLETO para ChatBot.Application/Features/Bot/Strategies/ExitCommandStrategy.cs

using ChatBot.Domain.ValueObjects;
using ChatBot.Application.Features.Bot.Commands.ProcessUserMessage;
using System.Threading.Tasks; // Adicionar este using

namespace ChatBot.Application.Features.Bot.Strategies;

/// <summary>
/// Estratgia de resposta para o comando de sada ("sair").
/// </summary>
public class ExitCommandStrategy : IBotResponseStrategy // Esta é a CLASSE que implementa a interface
{
    public bool CanHandle(ProcessUserMessageCommand command)
    {
        return command.UserMessage.ToLowerInvariant().Trim() == "sair";
    }

    public Task<MessageContent> GenerateResponse(ProcessUserMessageCommand command)
    {
        // Aqui voc pode adicionar lgica para encerrar a sesso de chat, se necessrio
        return Task.FromResult(MessageContent.Create("Sessão encerrada. Até a próxima!"));
    }
}