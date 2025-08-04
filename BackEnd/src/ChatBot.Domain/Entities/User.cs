using ChatBot.Domain.Entities;
using ChatBot.Domain.ValueObjects; // Necessário para Email

namespace ChatBot.Domain.Entities;

public class User : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public Email Email { get; set; } = null!; // Alterado de string para o Value Object Email
    public bool IsActive { get; set; } = true;

    // Navigation Properties
    public virtual ICollection<ChatSession> ChatSessions { get; set; } = new List<ChatSession>();
    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}