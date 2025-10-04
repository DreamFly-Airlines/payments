using Shared.Abstractions.IntegrationEvents;

namespace Payments.Application.Producers;

public interface IIntegrationEventProducer
{
    public Task ProduceAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IIntegrationEvent;
}