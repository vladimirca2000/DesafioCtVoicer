using ChatBot.Application.Common.Interfaces;
using ChatBot.Application.Common.Models; // Para usar Result<T>
using MediatR;

namespace ChatBot.Application.Features.Chat.ProcessBotResponse;

public record ProcessBotResponseCommand : ICommand<Result<ProcessBotResponseResponse>>
{
    public Guid ChatSessionId { get; init; }
}