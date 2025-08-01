using ChatBot.Application.Features.Bot.Strategies;

namespace ChatBot.Application.Features.Bot.Factories;

/// <summary>
/// Define o contrato para uma fábrica que seleciona a estratégia de resposta do bot mais adequada.
/// </summary>
public interface IBotResponseStrategyFactory
{
    /// <summary>
    /// Retorna a estratégia de resposta do bot mais adequada para a mensagem do usuário.
    /// </summary>
    /// <param name="userMessage">A mensagem enviada pelo usuário.</param>
    /// <returns>A estratégia de resposta do bot.</returns>
    IBotResponseStrategy GetStrategy(string userMessage);
}