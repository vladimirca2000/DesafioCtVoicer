using ChatBot.Application.Features.Bot.Commands.ProcessUserMessage;
using ChatBot.Application.Features.Bot.Strategies;

namespace ChatBot.Application.Features.Bot.Factories;

/// <summary>
/// Contrato para uma fábrica que retorna a estratégia de resposta do bot apropriada.
/// </summary>
public interface IBotResponseStrategyFactory
{
    Task<IBotResponseStrategy> GetStrategy(ProcessUserMessageCommand command);
}