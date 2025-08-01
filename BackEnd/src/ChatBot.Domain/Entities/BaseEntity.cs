using ChatBot.Domain.Interfaces;

namespace ChatBot.Domain.Entities;

public abstract class BaseEntity : ISoftDeletable, IAuditable
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? UpdatedBy { get; set; }

    // Propriedade para armazenar eventos de domínio (não será mapeada para o banco de dados)
    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// Adiciona um evento de domínio à lista.
    /// </summary>
    /// <param name="eventItem">O evento de domínio a ser adicionado.</param>
    public void AddDomainEvent(IDomainEvent eventItem)
    {
        _domainEvents.Add(eventItem);
    }

    /// <summary>
    /// Remove um evento de domínio da lista.
    /// </summary>
    /// <param name="eventItem">O evento de domínio a ser removido.</param>
    public void RemoveDomainEvent(IDomainEvent eventItem)
    {
        _domainEvents.Remove(eventItem);
    }

    /// <summary>
    /// Limpa todos os eventos de domínio da lista.
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
