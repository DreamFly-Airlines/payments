namespace Payments.Domain.Abstractions;

public abstract class AggregateRoot<TDomainEvent>
{
    public IReadOnlyCollection<TDomainEvent> DomainEvents => _events;
    private readonly List<TDomainEvent> _events = [];

    public void AddDomainEvent(TDomainEvent domainEvent) => _events.Add(domainEvent);
    
    public void ClearDomainEvents() => _events.Clear();
}