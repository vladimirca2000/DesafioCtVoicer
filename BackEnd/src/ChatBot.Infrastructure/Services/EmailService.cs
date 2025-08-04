using ChatBot.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ChatBot.Infrastructure.Services;

/// <summary>
/// Implementação concreta do serviço de envio de e-mails (simulado).
/// </summary>
public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    public Task SendEmailAsync(string to, string subject, string body)
    {
        // Esta é uma implementação mock. Em uma aplicação real, você usaria uma biblioteca
        // como MailKit ou um serviço de terceiros como SendGrid, Mailgun, etc.
        _logger.LogInformation("Simulando envio de e-mail para: {To} | Assunto: {Subject}", to, subject);
        _logger.LogDebug("Corpo do E-mail:\n{Body}", body);
        return Task.CompletedTask;
    }
}