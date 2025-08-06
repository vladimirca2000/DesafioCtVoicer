using ChatBot.Domain.Entities;
using ChatBot.Domain.ValueObjects; 
namespace ChatBot.Domain.Entities;

public class User : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public Email Email { get; set; } = null!; 
    public bool IsActive { get; set; } = true;

    
    public virtual ICollection<ChatSession> ChatSessions { get; set; } = new List<ChatSession>();
    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}