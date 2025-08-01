using MediatR; // Adicionar este using

namespace ChatBot.Domain.Interfaces;

/// <summary>
/// Interface base para eventos de domínio.
/// Herda de INotification para ser compatível com o MediatR.
/// </summary>
public interface IDomainEvent : INotification
{
    /// <summary>
    /// Data e hora em que o evento ocorreu (UTC).
    /// </summary>
    DateTime OccurredOn { get; }
}