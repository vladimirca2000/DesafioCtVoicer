using ChatBot.Domain.Enums;

namespace ChatBot.Domain.Entities;

public class Message : BaseEntity
{
    public Guid ChatSessionId { get; set; }
    public Guid? UserId { get; set; } // Null para mensagens do bot
    public string Content { get; set; } = string.Empty;
    public MessageType Type { get; set; }
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public bool IsFromBot { get; set; }

    // Navigation Properties
    public virtual ChatSession ChatSession { get; set; } = null!;
    public virtual User? User { get; set; }
}