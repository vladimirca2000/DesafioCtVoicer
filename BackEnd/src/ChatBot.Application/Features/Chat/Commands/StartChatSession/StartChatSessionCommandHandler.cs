using MediatR;
using ChatBot.Application.Common.Models;
using ChatBot.Domain.Entities;
using ChatBot.Domain.Repositories;
using ChatBot.Domain.Enums;
using ChatBot.Application.Common.Interfaces;
using ChatBot.Application.Common.Exceptions;
using ChatBot.Domain.ValueObjects; // Necessário para MessageContent

namespace ChatBot.Application.Features.Chat.Commands.StartChatSession;

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
                throw new NotFoundException("Usuário", request.UserId.Value);
            }
        }
        else // Criar um novo usuário se UserId não foi fornecido e UserName sim
        {
            user = new User
            {
                Name = request.UserName!, // UserName é garantido pelo validador se UserId for nulo
                Email = Email.Create($"{Guid.NewGuid()}@temp.com"), // Cria um Email VO para usuários anônimos
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

        // Criar o Value Object MessageContent para a mensagem inicial
        var initialMessageContent = MessageContent.Create(request.InitialMessageContent!);

        // Adicionar a mensagem inicial
        var initialMessage = new Message
        {
            ChatSessionId = chatSession.Id,
            UserId = user.Id,
            Content = initialMessageContent, // Usa o Value Object MessageContent
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
            InitialMessage = initialMessage.Content.Value // Retorna a string do conteúdo no DTO de resposta
        });
    }
}