using ChatBot.Domain.Enums; // Para BotResponseType

namespace ChatBot.Application.Features.Bot.Strategies;

/// <summary>
/// Estratégia de resposta para o comando de saída ("sair").
/// </summary>
public class ExitCommandStrategy : IBotResponseStrategy
{
    // Não precisamos de um repositório aqui, pois a lógica é simples e baseada na entrada.
    // Poderíamos injetar um IBotResponseRepository se tivéssemos uma resposta de saída pré-definida em DB.

    public async Task<string?> GenerateResponseAsync(string userMessage, CancellationToken cancellationToken)
    {
        // Converte a mensagem para minúsculas e remove espaços para uma comparação robusta.
        if (userMessage.Trim().Equals("sair", StringComparison.OrdinalIgnoreCase))
        {
            // Poderíamos ter uma mensagem de despedida configurável.
            // Por enquanto, uma simples string.
            return "Compreendido! A sessão de chat será encerrada. Até a próxima!";
        }

        // Se a mensagem não for o comando "sair", esta estratégia não se aplica.
        return null;
    }
}
