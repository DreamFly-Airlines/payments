using Payments.Application.Abstractions;
using Payments.Application.Producers;
using Payments.Domain.Events;

namespace Payments.Application.EventHandlers;

public class PaymentConfirmedEventHandler(
    IDomainEventProducer producer) : IDomainEventHandler<PaymentConfirmed>
{
    public async Task HandleAsync(PaymentConfirmed domainEvent, CancellationToken cancellationToken = default)
    {
        await producer.ProduceAsync(domainEvent,  cancellationToken);
    }
}