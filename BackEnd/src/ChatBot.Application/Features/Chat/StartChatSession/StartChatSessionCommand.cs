using MediatR;
using ChatBot.Application.Common.Interfaces;
using ChatBot.Application.Common.Models; // Para usar Result<T>

namespace ChatBot.Application.Features.Chat.StartChatSession;

public record StartChatSessionCommand : ICommand<Result<StartChatSessionResponse>>
{
    public Guid? UserId { get; init; } // Pode ser nulo se for um usuário anônimo
    public string? UserName { get; init; } // Nome do usuário, se não tiver um UserId
    public string? InitialMessageContent { get; init; } // Mensagem inicial da sessão
}