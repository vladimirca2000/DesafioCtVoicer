using MediatR;
using ChatBot.Domain.Events;
using ChatBot.Application.Features.Bot.Commands.ProcessUserMessage;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ChatBot.Application.Features.Chat.Commands.EventHandlers;

/// <summary>
/// Handler que automaticamente aciona o bot para responder quando uma mensagem do usuário é enviada.
/// </summary>
public class BotAutoResponseEventHandler : INotificationHandler<MessageSentDomainEvent>
{
    private readonly IMediator _mediator;
    private readonly ILogger<BotAutoResponseEventHandler> _logger;

    public BotAutoResponseEventHandler(IMediator mediator, ILogger<BotAutoResponseEventHandler> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Handle(MessageSentDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("=== INICIO BotAutoResponseEventHandler ===");
        _logger.LogInformation("MessageSentDomainEvent recebido: IsFromBot={IsFromBot}, Content='{Content}', ChatSessionId={ChatSessionId}", 
            notification.IsFromBot, notification.Content, notification.ChatSessionId);

        // Só processa se a mensagem NÃO for do bot (ou seja, é do usuário)
        if (notification.IsFromBot)
        {
            _logger.LogInformation("? Mensagem é do bot. Ignorando auto-resposta para evitar loop infinito.");
            return;
        }

        _logger.LogInformation("? Mensagem de usuário detectada. Acionando resposta automática do bot para sessão {ChatSessionId}", notification.ChatSessionId);

        try
        {
            // Criar comando para o bot processar a mensagem do usuário
            var processCommand = new ProcessUserMessageCommand
            {
                ChatSessionId = notification.ChatSessionId,
                UserId = notification.UserId ?? Guid.Empty, // Tratar UserId nullable
                UserMessage = notification.Content
            };

            _logger.LogInformation("?? Enviando ProcessUserMessageCommand: UserId={UserId}, UserMessage='{UserMessage}'", 
                processCommand.UserId, processCommand.UserMessage);

            // Enviar comando para o bot processar
            var result = await _mediator.Send(processCommand, cancellationToken);

            if (result.IsSuccess)
            {
                _logger.LogInformation("? Bot respondeu com sucesso para a mensagem na sessão {ChatSessionId}. Resposta ID: {MessageId}, Conteúdo: '{Content}'", 
                    notification.ChatSessionId, result.Value?.MessageId, result.Value?.BotMessageContent);
            }
            else
            {
                _logger.LogWarning("? Bot falhou ao responder para a mensagem na sessão {ChatSessionId}. Erros: {Errors}", 
                    notification.ChatSessionId, string.Join(", ", result.Errors));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "?? Erro ao processar resposta automática do bot para sessão {ChatSessionId}", notification.ChatSessionId);
        }
        
        _logger.LogInformation("=== FIM BotAutoResponseEventHandler ===");
    }
}