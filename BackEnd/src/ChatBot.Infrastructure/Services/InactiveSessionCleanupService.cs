using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using ChatBot.Domain.Repositories;
using ChatBot.Domain.Enums;
using ChatBot.Domain.Entities;
using ChatBot.Application.Features.Bot.Commands.ProcessUserMessage;
using MediatR;
using ChatBot.Application.Common.Interfaces;

namespace ChatBot.Infrastructure.Services;

public class InactiveSessionCleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<InactiveSessionCleanupService> _logger;
    private static readonly TimeSpan InactivityLimit = TimeSpan.FromHours(2);
    private static readonly TimeSpan CheckInterval = TimeSpan.FromMinutes(5);

    public InactiveSessionCleanupService(IServiceProvider serviceProvider, ILogger<InactiveSessionCleanupService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var chatSessionRepo = scope.ServiceProvider.GetRequiredService<IChatSessionRepository>();
                    var messageRepo = scope.ServiceProvider.GetRequiredService<IMessageRepository>();
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                    var signalR = scope.ServiceProvider.GetService<ISignalRChatService>();

                    var now = DateTime.UtcNow;
                    var sessions = await chatSessionRepo.GetActiveSessionsAsync(stoppingToken);

                    foreach (var session in sessions)
                    {
                        var lastMessage = await messageRepo.GetLastMessageInSessionAsync(session.Id, stoppingToken);
                        if (lastMessage == null) continue;
                        if (now - lastMessage.SentAt > InactivityLimit)
                        {
                            // Encerrar sessão
                            session.Status = SessionStatus.Ended;
                            session.EndedAt = now;
                            session.UpdatedAt = now;
                            session.UpdatedBy = "System";
                            await chatSessionRepo.UpdateAsync(session, stoppingToken);

                            // Enviar mensagem de saída
                            var exitCommand = new ProcessUserMessageCommand
                            {
                                ChatSessionId = session.Id,
                                UserId = Guid.Empty, // Bot
                                UserMessage = "sair"
                            };
                            await mediator.Send(exitCommand, stoppingToken);

                            // Notificar o front via SignalR (se disponível)
                            if (signalR != null)
                            {
                                await signalR.NotifyChatSessionEnded(session.Id, "Sessão encerrada por inatividade.");
                            }

                            _logger.LogInformation($"Sessão {session.Id} encerrada por inatividade.");
                        }
                    }
                    await unitOfWork.SaveChangesAsync(stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao encerrar sessões inativas.");
            }
            await Task.Delay(CheckInterval, stoppingToken);
        }
    }
}
