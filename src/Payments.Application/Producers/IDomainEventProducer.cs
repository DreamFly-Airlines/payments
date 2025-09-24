using Payments.Domain.Abstractions;

namespace Payments.Application.Producers;

public interface IDomainEventProducer
{
    public Task ProduceAsync<TEvent>(
        TEvent @event, CancellationToken cancellationToken = default) 
        where TEvent : IDomainEvent;
}