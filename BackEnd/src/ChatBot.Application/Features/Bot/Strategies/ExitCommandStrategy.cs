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
    public async Task<bool> CanHandle(ProcessUserMessageCommand command)
    {
        // Exemplo: comando de saída detectado
        var message = command.UserMessage.ToLowerInvariant();
        return await Task.FromResult(message.Contains("sair") || message.Contains("encerrar"));
    }

    public Task<MessageContent> GenerateResponse(ProcessUserMessageCommand command)
    {
        // Aqui voc pode adicionar lgica para encerrar a sesso de chat, se necessrio
        return Task.FromResult(MessageContent.Create("Sessão encerrada. Até a próxima!"));
    }
}