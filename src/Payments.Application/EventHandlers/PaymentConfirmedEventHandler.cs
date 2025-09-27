using Payments.Application.Abstractions;
using Payments.Application.Exceptions;
using Payments.Application.Producers;
using Payments.Domain.AggregateRoots;
using Payments.Domain.Events;
using Payments.Domain.Repositories;

namespace Payments.Application.EventHandlers;

public class PaymentConfirmedEventHandler(
    IPaymentRepository paymentRepository,
    IEventProducer producer) : IEventHandler<PaymentConfirmed>
{
    public async Task HandleAsync(PaymentConfirmed @event, CancellationToken cancellationToken = default)
    {
        var payment = await paymentRepository.GetByIdAsync(@event.PaymentId, cancellationToken);
        if (payment is null)
            throw new NotFoundException(nameof(Payment), @event.PaymentId);
        await producer.ProduceAsync(@event, cancellationToken);
    }
}