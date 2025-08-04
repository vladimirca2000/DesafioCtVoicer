using System.Threading.Tasks;

namespace ChatBot.Application.Common.Interfaces;

/// <summary>
/// Contrato para o serviço de envio de e-mails.
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Envia um e-mail assincronamente.
    /// </summary>
    /// <param name="to">Endereço de e-mail do destinatário.</param>
    /// <param name="subject">Assunto do e-mail.</param>
    /// <param name="body">Corpo do e-mail.</param>
    Task SendEmailAsync(string to, string subject, string body);
}