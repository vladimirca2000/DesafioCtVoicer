namespace ChatBot.Application.Features.Bot.Strategies;

/// <summary>
/// Define o contrato para diferentes estratégias de resposta do bot.
/// </summary>
public interface IBotResponseStrategy
{
    /// <summary>
    /// Processa a mensagem do usuário e gera uma resposta do bot, se aplicável.
    /// </summary>
    /// <param name="userMessage">A mensagem enviada pelo usuário.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>A resposta gerada pelo bot, ou null/vazio se esta estratégia não se aplica.</returns>
    Task<string?> GenerateResponseAsync(string userMessage, CancellationToken cancellationToken);
}
