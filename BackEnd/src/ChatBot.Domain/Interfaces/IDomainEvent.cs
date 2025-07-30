namespace ChatBot.Domain.Interfaces;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}