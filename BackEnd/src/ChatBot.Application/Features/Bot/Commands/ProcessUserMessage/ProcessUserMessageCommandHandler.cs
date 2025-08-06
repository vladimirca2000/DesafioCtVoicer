// Conteúdo COMPLETO para ChatBot.Application/Features/Bot/Commands/ProcessUserMessage/ProcessUserMessageCommandHandler.cs

using MediatR;
using ChatBot.Application.Common.Models;
using ChatBot.Application.Common.Interfaces;
using ChatBot.Application.Features.Bot.Factories;
using ChatBot.Domain.Repositories;
using ChatBot.Domain.Entities;
using ChatBot.Domain.Enums;
using ChatBot.Domain.ValueObjects; // Necessário para MessageContent
using ChatBot.Application.Common.Exceptions; // Para NotFoundException

namespace ChatBot.Application.Features.Bot.Commands.ProcessUserMessage;

/// <summary>
/// Manipulador para o comando ProcessUserMessageCommand.
/// Orquestra a lgica de resposta do bot.
/// </summary>
public class ProcessUserMessageCommandHandler : IRequestHandler<ProcessUserMessageCommand, Result<ProcessUserMessageResponse>>
{
    private readonly IBotResponseStrategyFactory _botResponseStrategyFactory;
    private readonly IMessageRepository _messageRepository;
    private readonly IChatSessionRepository _chatSessionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProcessUserMessageCommandHandler(
        IBotResponseStrategyFactory botResponseStrategyFactory,
        IMessageRepository messageRepository,
        IChatSessionRepository chatSessionRepository,
        IUnitOfWork unitOfWork)
    {
        _botResponseStrategyFactory = botResponseStrategyFactory;
        _messageRepository = messageRepository;
        _chatSessionRepository = chatSessionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ProcessUserMessageResponse>> Handle(ProcessUserMessageCommand request, CancellationToken cancellationToken)
    {
        // 1. Validar se a sessão de chat existe
        var chatSession = await _chatSessionRepository.GetByIdAsync(request.ChatSessionId, cancellationToken);
        if (chatSession == null)
        {
            throw new NotFoundException("Sessão de chat não encontrada.");
        }

        // 2. Selecionar a estratégia de resposta do bot (agora assíncrona)
        var strategy = await _botResponseStrategyFactory.GetStrategy(request);

        // 3. Gerar o conteúdo da resposta do bot usando a estratégia selecionada
        var botResponseContent = await strategy.GenerateResponse(request);

        // 4. Criar a entidade Message para a resposta do bot
        var botMessage = new Message
        {
            ChatSessionId = request.ChatSessionId,
            UserId = null, // Mensagem do bot, sem UserId associado diretamente
            Content = botResponseContent, // Conteúdo já é MessageContent
            Type = MessageType.BotResponse,
            IsFromBot = true,
            SentAt = DateTime.UtcNow,
            CreatedBy = "BotSystem"
        };

        // 5. Adicionar a mensagem do bot ao repositório
        await _messageRepository.AddAsync(botMessage, cancellationToken);

        // 6. Salvar as mudanças
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 7. Retornar a resposta
        return Result<ProcessUserMessageResponse>.Success(new ProcessUserMessageResponse
        {
            MessageId = botMessage.Id,
            ChatSessionId = botMessage.ChatSessionId,
            BotMessageContent = botMessage.Content.Value,
            SentAt = botMessage.SentAt
        });
    }
}