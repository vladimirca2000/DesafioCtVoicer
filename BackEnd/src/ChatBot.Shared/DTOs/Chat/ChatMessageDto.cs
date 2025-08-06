namespace ChatBot.Shared.DTOs.Chat;

/// <summary>
/// DTO para representar uma mensagem de chat.
/// </summary>
public class ChatMessageDto
{
    public Guid Id { get; set; }
    public Guid ChatSessionId { get; set; }
    public Guid? UserId { get; set; } 
    public string Content { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
    public bool IsFromBot { get; set; }
}
