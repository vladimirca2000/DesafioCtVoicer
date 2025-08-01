using ChatBot.Application.Features.Bot.Strategies;
using Microsoft.Extensions.Logging; // Para logar a decisão da fábrica

namespace ChatBot.Application.Features.Bot.Factories;

/// <summary>
/// Implementação concreta da fábrica de estratégias de resposta do bot.
/// Responsável por selecionar a estratégia mais adequada com base na mensagem do usuário.
/// </summary>
public class BotResponseStrategyFactory : IBotResponseStrategyFactory
{
    private readonly IEnumerable<IBotResponseStrategy> _strategies;
    private readonly ILogger<BotResponseStrategyFactory> _logger;

    public BotResponseStrategyFactory(IEnumerable<IBotResponseStrategy> strategies,
                                      ILogger<BotResponseStrategyFactory> logger)
    {
        _strategies = strategies;
        _logger = logger;
    }

    public IBotResponseStrategy GetStrategy(string userMessage)
    {
        // Ordem de prioridade para seleção da estratégia:
        // 1. ExitCommandStrategy (se o comando "sair" for detectado)
        // 2. KeywordBasedResponseStrategy (se houver palavras-chave correspondentes)
        // 3. RandomResponseStrategy (como fallback padrão)

        // Tentar encontrar a ExitCommandStrategy
        var exitStrategy = _strategies.OfType<ExitCommandStrategy>().FirstOrDefault();
        if (exitStrategy != null && userMessage.Trim().Equals("sair", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogInformation("Fábrica selecionou: ExitCommandStrategy");
            return exitStrategy;
        }

        // Tentar encontrar a KeywordBasedResponseStrategy (se o usuário não pediu para sair)
        var keywordStrategy = _strategies.OfType<KeywordBasedResponseStrategy>().FirstOrDefault();
        // NOTA: A KeywordBasedResponseStrategy precisa ser capaz de retornar null se não encontrar palavras-chave.
        // A decisão de qual estratégia usar é da fábrica. A estratégia apenas executa seu comportamento.
        if (keywordStrategy != null)
        //if (keywordStrategy != null && keywordStrategy.CanHandle(userMessage)) 
        {
            // Poderíamos adicionar aqui uma pré-verificação mais inteligente se a mensagem não for "sair".
            // Por simplicidade, a GetStrategy vai retornar a KeywordBasedStrategy e ela mesma decide se aplica ou não.
            // Para uma decisão mais robusta na fábrica, a GetStrategy na IBotResponseStrategy
            // poderia ter um método 'bool CanHandle(string userMessage)'
            // Para manter a consistência com o que já fizemos e evitar mudanças nas interfaces,
            // vamos apenas retornar as estratégias na ordem de prioridade.
            // A responsabilidade de 'realmente' responder (retornar string ou null) fica com o GenerateResponseAsync.

            _logger.LogInformation("Fábrica selecionou: KeywordBasedResponseStrategy");
            return keywordStrategy; // Retornamos a estratégia, ela mesma decidirá se tem uma resposta.
        }

        // Se nenhuma das anteriores se aplicar, retornar a RandomResponseStrategy como fallback
        var randomStrategy = _strategies.OfType<RandomResponseStrategy>().FirstOrDefault();
        if (randomStrategy == null)
        {
            _logger.LogError("RandomResponseStrategy não encontrada. O sistema pode ficar sem respostas padrão.");
            throw new InvalidOperationException("RandomResponseStrategy não está registrada ou disponível.");
        }

        _logger.LogInformation("Fábrica selecionou: RandomResponseStrategy (fallback)");
        return randomStrategy;
    }
}