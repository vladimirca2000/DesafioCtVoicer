namespace ChatBot.Shared.DTOs.General;

/// <summary>
/// DTO padrão para respostas de erro da API.
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// Um breve título para a natureza do erro.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// O código de status HTTP do erro (e.g., 400, 404, 500).
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// Uma descrição detalhada do erro.
    /// </summary>
    public string? Detail { get; set; }

    /// <summary>
    /// Dicionário de erros de validação (para erros 400 Bad Request, onde a chave é o nome do campo e o valor são as mensagens de erro).
    /// </summary>
    public IDictionary<string, string[]>? Errors { get; set; }

    /// <summary>
    /// Lista plana de mensagens de erro, útil para exibir ao usuário.
    /// </summary>
    public List<string>? Messages { get; set; }
}