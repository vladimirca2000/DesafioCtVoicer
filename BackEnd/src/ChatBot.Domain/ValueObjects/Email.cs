// C:\Desenvolvimento\DocChatBoot\BackEnd\src\ChatBot.Domain\ValueObjects\Email.cs

using System.Net.Mail; // Para validação de formato de e-mail
using System; // Necessário para ArgumentException

namespace ChatBot.Domain.ValueObjects;

/// <summary>
/// Value Object que representa um endereço de e-mail.
/// Garante que o e-mail seja válido no momento da criação e que seja normalizado.
/// </summary>
public record Email
{
    public string Value { get; private set; }

    /// <summary>
    /// Construtor privado para garantir que a instância só possa ser criada
    /// através do método de fábrica ou com validação interna.
    /// </summary>
    private Email(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Cria uma nova instância de Email após validar o formato e normalizar para minúsculas.
    /// </summary>
    /// <param name="email">O endereço de e-mail a ser validado e encapsulado.</param>
    /// <returns>Uma nova instância de Email.</returns>
    /// <exception cref="ArgumentException">Lançada se o formato do e-mail for inválido ou vazio.</exception>
    public static Email Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("O endereço de e-mail não pode ser vazio ou nulo.", nameof(email));
        }
        try
        {
            var mailAddress = new MailAddress(email);
            // Correção aplicada: Normaliza o e-mail para minúsculas antes de armazenar.
            return new Email(email.ToLowerInvariant());
        }
        catch (FormatException)
        {
            throw new ArgumentException($"O formato do e-mail '{email}' é inválido.", nameof(email));
        }
    }

    /// <summary>
    /// Permite a conversão implícita de Email para string (retorna o valor).
    /// </summary>
    public static implicit operator string(Email email) => email.Value;

    /// <summary>
    /// Permite a conversão explícita de string para Email (cria um novo Email).
    /// </summary>
    public static explicit operator Email(string email) => Create(email);

    public override string ToString() => Value;
}