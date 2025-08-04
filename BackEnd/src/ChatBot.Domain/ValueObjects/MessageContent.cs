namespace ChatBot.Domain.ValueObjects;

/// <summary>
/// Value Object que representa o conteúdo de uma mensagem.
/// Garante que o conteúdo seja válido (e.g., tamanho) no momento da criação.
/// </summary>
public record MessageContent
{
    public string Value { get; private set; }

    // Definir limites para o conteúdo da mensagem
    private const int MinLength = 1;
    private const int MaxLength = 2000; // Conforme definido na configuração do EF Core

    /// <summary>
    /// Construtor privado para garantir que a instância só possa ser criada
    /// através do método de fábrica ou com validação interna.
    /// </summary>
    private MessageContent(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Cria uma nova instância de MessageContent após validar o conteúdo.
    /// </summary>
    /// <param name="content">O conteúdo da mensagem a ser validado e encapsulado.</param>
    /// <returns>Uma nova instância de MessageContent.</returns>
    /// <exception cref="ArgumentException">Lançada se o conteúdo for inválido.</exception>
    public static MessageContent Create(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            throw new ArgumentException($"O conteúdo da mensagem não pode ser vazio.", nameof(content));
        }

        if (content.Length < MinLength || content.Length > MaxLength)
        {
            throw new ArgumentException($"O conteúdo da mensagem deve ter entre {MinLength} e {MaxLength} caracteres. Atual: {content.Length}", nameof(content));
        }

        return new MessageContent(content);
    }

    /// <summary>
    /// Permite a conversão implícita de MessageContent para string (retorna o valor).
    /// </summary>
    public static implicit operator string(MessageContent content) => content.Value;

    /// <summary>
    /// Permite a conversão explícita de string para MessageContent (cria um novo MessageContent).
    /// </summary>
    public static explicit operator MessageContent(string content) => Create(content);

    public override string ToString() => Value;
}