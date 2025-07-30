using ChatBot.Domain.Enums;

namespace ChatBot.Domain.Entities;

public class BotResponse : BaseEntity
{
    public string Content { get; set; } = string.Empty;
    public BotResponseType Type { get; set; }
    public string? Keywords { get; set; } // JSON array de palavras-chave
    public bool IsActive { get; set; } = true;
    public int Priority { get; set; } = 0; // Para ordenação
}