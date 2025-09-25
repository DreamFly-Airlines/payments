using Payments.Domain.Abstractions;

namespace Payments.Application.Producers;

public interface IEventProducer
{
    public Task ProduceAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default);
}