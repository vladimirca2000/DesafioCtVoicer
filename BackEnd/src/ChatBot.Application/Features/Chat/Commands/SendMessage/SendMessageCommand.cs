using MediatR;
using ChatBot.Application.Common.Interfaces;
using ChatBot.Application.Common.Models;
using ChatBot.Domain.Enums;

namespace ChatBot.Application.Features.Chat.Commands.SendMessage;

public record SendMessageCommand : ICommand<Result<SendMessageResponse>>
{
    public Guid ChatSessionId { get; init; }
    public Guid UserId { get; init; } // Quem está enviando a mensagem
    public string Content { get; init; } = string.Empty; // Permanece string para input da API
    public MessageType MessageType { get; init; } = MessageType.Text; // Tipo da mensagem (texto, comando, etc.)
}