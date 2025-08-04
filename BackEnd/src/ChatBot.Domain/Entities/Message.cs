using ChatBot.Domain.Enums;
using ChatBot.Domain.ValueObjects; // Necessário para MessageContent

namespace ChatBot.Domain.Entities;

public class Message : BaseEntity
{
    public Guid ChatSessionId { get; set; }
    public Guid? UserId { get; set; } // Null para mensagens do bot
    public MessageContent Content { get; set; } = null!; // Alterado de string para o Value Object MessageContent
    public MessageType Type { get; set; }
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public bool IsFromBot { get; set; }

    // Navigation Properties
    public virtual ChatSession ChatSession { get; set; } = null!;
    public virtual User? User { get; set; }
}