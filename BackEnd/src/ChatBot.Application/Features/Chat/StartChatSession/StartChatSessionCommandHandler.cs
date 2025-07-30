using MediatR;
using ChatBot.Application.Common.Models;
using ChatBot.Domain.Entities;
using ChatBot.Domain.Repositories;
using ChatBot.Domain.Enums;
using ChatBot.Application.Common.Interfaces; // Para IUnitOfWork

namespace ChatBot.Application.Features.Chat.StartChatSession;

public class StartChatSessionCommandHandler : IRequestHandler<StartChatSessionCommand, Result<StartChatSessionResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IChatSessionRepository _chatSessionRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly IUnitOfWork _unitOfWork;

    public StartChatSessionCommandHandler(
        IUserRepository userRepository,
        IChatSessionRepository chatSessionRepository,
        IMessageRepository messageRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _chatSessionRepository = chatSessionRepository;
        _messageRepository = messageRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<StartChatSessionResponse>> Handle(StartChatSessionCommand request, CancellationToken cancellationToken)
    {
        User user;

        // Tentar encontrar o usuário existente ou criar um novo
        if (request.UserId.HasValue)
        {
            user = await _userRepository.GetByIdAsync(request.UserId.Value, cancellationToken);
            if (user == null)
            {
                return Result<StartChatSessionResponse>.Failure("Usuário não encontrado.");
            }
        }
        else // Criar um novo usuário se UserId não foi fornecido e UserName sim
        {
            user = new User
            {
                Name = request.UserName!, // UserName é garantido pelo validador se UserId for nulo
                Email = $"{Guid.NewGuid()}@temp.com", // Email temporário para usuários anônimos
                IsActive = true
            };
            await _userRepository.AddAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken); // Salvar usuário para obter o ID
        }

        // Criar a nova sessão de chat
        var chatSession = new ChatSession
        {
            UserId = user.Id,
            Status = SessionStatus.Active,
            StartedAt = DateTime.UtcNow,
            CreatedBy = user.Name // ou de um contexto de usuário real
        };
        await _chatSessionRepository.AddAsync(chatSession, cancellationToken);

        // Adicionar a mensagem inicial
        var initialMessage = new Message
        {
            ChatSessionId = chatSession.Id,
            UserId = user.Id,
            Content = request.InitialMessageContent!, // Conteúdo garantido pelo validador
            Type = MessageType.Text,
            IsFromBot = false,
            SentAt = DateTime.UtcNow,
            CreatedBy = user.Name // ou de um contexto de usuário real
        };
        await _messageRepository.AddAsync(initialMessage, cancellationToken);

        // Salvar todas as mudanças na transação
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Retornar a resposta
        return Result<StartChatSessionResponse>.Success(new StartChatSessionResponse
        {
            ChatSessionId = chatSession.Id,
            UserId = user.Id,
            StartedAt = chatSession.StartedAt,
            InitialMessage = initialMessage.Content
        });
    }
}