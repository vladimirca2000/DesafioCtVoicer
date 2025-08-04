using MediatR; // Necessário para INotification

namespace ChatBot.Domain.Interfaces;

/// <summary>
/// Contrato base para todos os eventos de domínio.
/// Deve herdar de INotification para ser compatível com MediatR.
/// </summary>
public interface IDomainEvent : INotification
{
    /// <summary>
    /// Data e hora em que o evento ocorreu.
    /// </summary>
    DateTime OccurredOn { get; }
}