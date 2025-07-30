using MediatR;
using ChatBot.Application.Common.Models;
using ChatBot.Domain.Entities;
using ChatBot.Domain.Repositories;
using ChatBot.Domain.Enums;
using ChatBot.Application.Common.Interfaces; // Para IUnitOfWork

namespace ChatBot.Application.Features.Chat.ProcessBotResponse;

public class ProcessBotResponseCommandHandler : IRequestHandler<ProcessBotResponseCommand, Result<ProcessBotResponseResponse>>
{
    private readonly IChatSessionRepository _chatSessionRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly IBotResponseRepository _botResponseRepository; // Novo repositório para respostas do bot
    private readonly IUnitOfWork _unitOfWork;

    public ProcessBotResponseCommandHandler(
        IChatSessionRepository chatSessionRepository,
        IMessageRepository messageRepository,
        IBotResponseRepository botResponseRepository,
        IUnitOfWork unitOfWork)
    {
        _chatSessionRepository = chatSessionRepository;
        _messageRepository = messageRepository;
        _botResponseRepository = botResponseRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ProcessBotResponseResponse>> Handle(ProcessBotResponseCommand request, CancellationToken cancellationToken)
    {
        // 1. Validar se a sessão de chat existe
        var chatSession = await _chatSessionRepository.GetByIdAsync(request.ChatSessionId, cancellationToken);
        if (chatSession == null)
        {
            return Result<ProcessBotResponseResponse>.Failure("Sessão de chat não encontrada.");
        }

        // 2. Simular a lógica do bot para gerar uma resposta
        // Aqui, podemos usar regras simples ou integrar com um serviço de IA mais complexo
        string botResponseContent = await GetBotResponse(request.ChatSessionId, cancellationToken);

        // 3. Criar a mensagem do bot
        var botMessage = new Message
        {
            ChatSessionId = request.ChatSessionId,
            UserId = chatSession.UserId, // Bot responde ao usuário da sessão
            Content = botResponseContent,
            Type = MessageType.Text,
            IsFromBot = true,
            SentAt = DateTime.UtcNow,
            CreatedBy = "Bot" // Identificar a mensagem como vinda do bot
        };

        // 4. Adicionar a mensagem ao repositório
        await _messageRepository.AddAsync(botMessage, cancellationToken);

        // 5. Salvar todas as mudanças no banco de dados
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 6. Retornar a resposta
        return Result<ProcessBotResponseResponse>.Success(new ProcessBotResponseResponse
        {
            MessageId = botMessage.Id,
            ChatSessionId = botMessage.ChatSessionId,
            Content = botMessage.Content,
            SentAt = botMessage.SentAt
        });
    }

    private async Task<string> GetBotResponse(Guid chatSessionId, CancellationToken cancellationToken)
    {
        // Simulação simples: buscar uma resposta aleatória do repositório de BotResponse
        var responses = await _botResponseRepository.GetAllAsync(cancellationToken);
        if (responses == null || !responses.Any())
        {
            return "Desculpe, não tenho respostas configuradas no momento.";
        }

        // Selecionar uma resposta aleatória
        var random = new Random();
        var index = random.Next(responses.Count());
        return responses.ElementAt(index).Content;
    }
}