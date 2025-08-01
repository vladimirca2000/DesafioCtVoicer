using MediatR;
using ChatBot.Application.Common.Interfaces;
using ChatBot.Application.Common.Models;
using ChatBot.Application.Features.Bot.Factories;
using ChatBot.Domain.Repositories;
using ChatBot.Domain.Entities;
using ChatBot.Domain.Enums;
using ChatBot.Domain.Events;

namespace ChatBot.Application.Features.Bot.Commands.ProcessUserMessage;

// Remover as definições de 'ProcessUserMessageCommand' e 'ProcessUserMessageResponse' deste arquivo.

public class ProcessUserCommandValidator // << ATENÇÃO: Renomeie esta classe para ProcessUserMessageCommandValidator
{
    // ... conteúdo de validação, se houver.
    // Se ainda não tiver um validador, ele pode ser um arquivo separado: ProcessUserMessageCommandValidator.cs
}

public class ProcessUserMessageCommandHandler : IRequestHandler<ProcessUserMessageCommand, Result<ProcessUserMessageResponse>>
{
    private readonly IBotResponseStrategyFactory _strategyFactory;
    private readonly IMessageRepository _messageRepository;
    private readonly IChatSessionRepository _chatSessionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProcessUserMessageCommandHandler(
        IBotResponseStrategyFactory strategyFactory,
        IMessageRepository messageRepository,
        IChatSessionRepository chatSessionRepository,
        IUnitOfWork unitOfWork)
    {
        _strategyFactory = strategyFactory;
        _messageRepository = messageRepository;
        _chatSessionRepository = chatSessionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ProcessUserMessageResponse>> Handle(ProcessUserMessageCommand request, CancellationToken cancellationToken)
    {
        var chatSession = await _chatSessionRepository.GetByIdAsync(request.ChatSessionId, cancellationToken);
        if (chatSession == null)
        {
            return Result<ProcessUserMessageResponse>.Failure("Sessão de chat não encontrada para processamento do bot.");
        }

        var strategy = _strategyFactory.GetStrategy(request.UserMessageContent);
        var botResponseContent = await strategy.GenerateResponseAsync(request.UserMessageContent, cancellationToken);

        if (string.IsNullOrWhiteSpace(botResponseContent))
        {
            botResponseContent = "Desculpe, não entendi a sua solicitação. Poderia reformular?";
        }

        var botMessage = new Message
        {
            ChatSessionId = request.ChatSessionId,
            UserId = null,
            Content = botResponseContent,
            Type = MessageType.BotResponse,
            IsFromBot = true,
            SentAt = DateTime.UtcNow,
            CreatedBy = "Bot"
        };

        await _messageRepository.AddAsync(botMessage, cancellationToken);

        botMessage.AddDomainEvent(new MessageSentDomainEvent(
            botMessage.Id,
            botMessage.ChatSessionId,
            botMessage.UserId,
            botMessage.Content,
            botMessage.SentAt,
            botMessage.IsFromBot
        ));

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<ProcessUserMessageResponse>.Success(new ProcessUserMessageResponse
        {
            MessageId = botMessage.Id,
            ChatSessionId = botMessage.ChatSessionId,
            BotResponseContent = botMessage.Content,
            SentAt = botMessage.SentAt
        });
    }
}