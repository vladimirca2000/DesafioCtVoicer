using MediatR;
using ChatBot.Application.Common.Models;
using ChatBot.Domain.Entities;
using ChatBot.Domain.Repositories;
using ChatBot.Application.Common.Exceptions; // Para NotFoundException
using ChatBot.Application.Common.Interfaces;

namespace ChatBot.Application.Features.Chat.Commands.SendMessage;

public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, Result<SendMessageResponse>>
{
    private readonly IChatSessionRepository _chatSessionRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly IUserRepository _userRepository; // Para validar se o usuário existe
    private readonly IUnitOfWork _unitOfWork;

    public SendMessageCommandHandler(
        IChatSessionRepository chatSessionRepository,
        IMessageRepository messageRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _chatSessionRepository = chatSessionRepository;
        _messageRepository = messageRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<SendMessageResponse>> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        // 1. Validar se a sessão de chat existe
        var chatSession = await _chatSessionRepository.GetByIdAsync(request.ChatSessionId, cancellationToken);
        if (chatSession == null)
        {
            // Poderíamos lançar uma NotFoundException que seria capturada pelo middleware,
            // ou retornar um Result<T> de falha. Vou usar Result<T> para consistência com o que já fizemos.
            return Result<SendMessageResponse>.Failure("Sessão de chat não encontrada.");
        }

        // 2. Validar se o usuário existe
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            return Result<SendMessageResponse>.Failure("Usuário remetente não encontrado.");
        }

        // Se a sessão estiver encerrada, não permitir o envio de mensagens (opcional, dependendo da regra de negócio)
        if (chatSession.Status != Domain.Enums.SessionStatus.Active)
        {
            return Result<SendMessageResponse>.Failure("Não é possível enviar mensagens para uma sessão de chat inativa.");
        }

        // 3. Criar a nova mensagem
        var message = new Message
        {
            ChatSessionId = request.ChatSessionId,
            UserId = request.UserId,
            Content = request.Content,
            Type = request.MessageType,
            IsFromBot = false, // Esta mensagem é do usuário, não do bot
            SentAt = DateTime.UtcNow,
            CreatedBy = user.Name // Usar o nome do usuário para auditoria
        };

        // 4. Adicionar a mensagem ao repositório
        await _messageRepository.AddAsync(message, cancellationToken);

        // 5. Atualizar a sessão para indicar atividade recente (opcional, mas bom para tracking)
        // Você pode adicionar uma propriedade LastActivityAt na ChatSession
        // chatSession.LastActivityAt = DateTime.UtcNow;
        // await _chatSessionRepository.UpdateAsync(chatSession, cancellationToken); // O interceptor já atualiza UpdatedAt

        // 6. Salvar todas as mudanças no banco de dados através da UnitOfWork
        // O TransactionBehavior garante a transação para comandos, então SaveChangesAsync basta aqui.
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 7. Retornar a resposta de sucesso
        return Result<SendMessageResponse>.Success(new SendMessageResponse
        {
            MessageId = message.Id,
            ChatSessionId = message.ChatSessionId,
            UserId = message.UserId.Value, // Assumindo que UserId nunca será nulo aqui
            Content = message.Content,
            SentAt = message.SentAt
        });
    }
}