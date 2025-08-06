using ChatBot.Domain.Enums;
using ChatBot.Domain.ValueObjects; 

namespace ChatBot.Domain.Entities;

public class Message : BaseEntity
{
    public Guid ChatSessionId { get; set; }
    public Guid? UserId { get; set; } 
    public MessageContent Content { get; set; } = null!; 
    public MessageType Type { get; set; }
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public bool IsFromBot { get; set; }

    
    public virtual ChatSession ChatSession { get; set; } = null!;
    public virtual User? User { get; set; }
}