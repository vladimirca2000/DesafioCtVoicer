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
        // 1. Validar se a sesso de chat existe
        var chatSession = await _chatSessionRepository.GetByIdAsync(request.ChatSessionId, cancellationToken);
        if (chatSession == null)
        {
            throw new NotFoundException("Sesso de chat no encontrada.");
        }

        // 2. Selecionar a estratgia de resposta do bot
        var strategy = _botResponseStrategyFactory.GetStrategy(request);

        // 3. Gerar o contedo da resposta do bot usando a estratgia selecionada
        // ALTERADO: Adicionado await na chamada de GenerateResponse
        var botResponseContent = await strategy.GenerateResponse(request);

        // 4. Criar a entidade Message para a resposta do bot
        var botMessage = new Message
        {
            ChatSessionId = request.ChatSessionId,
            UserId = null, // Mensagem do bot, sem UserId associado diretamente
            Content = botResponseContent, // Contedo j  MessageContent, no precisa de converso explcita
            Type = MessageType.BotResponse,
            IsFromBot = true,
            SentAt = DateTime.UtcNow,
            CreatedBy = "BotSystem" // Usar um nome de auditoria para o bot
        };

        // 5. Adicionar a mensagem do bot ao repositrio
        await _messageRepository.AddAsync(botMessage, cancellationToken);

        // 6. Salvar as mudanas
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 7. Retornar a resposta
        return Result<ProcessUserMessageResponse>.Success(new ProcessUserMessageResponse
        {
            MessageId = botMessage.Id,
            ChatSessionId = botMessage.ChatSessionId,
            BotMessageContent = botMessage.Content.Value, // Acessa o valor string do MessageContent para o DTO
            SentAt = botMessage.SentAt
        });
    }
}