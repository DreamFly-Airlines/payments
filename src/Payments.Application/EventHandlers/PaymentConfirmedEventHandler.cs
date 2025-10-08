using Payments.Application.Exceptions;
using Payments.Domain.AggregateRoots;
using Payments.Domain.Events;
using Payments.Domain.Repositories;
using Shared.Abstractions.Events;
using Shared.Abstractions.IntegrationEvents;
using Shared.IntegrationEvents.Payments;

namespace Payments.Application.EventHandlers;

public class PaymentConfirmedEventHandler(
    IPaymentRepository paymentRepository,
    IIntegrationEventPublisher publisher) : IEventHandler<PaymentConfirmed>
{
    public async Task HandleAsync(PaymentConfirmed @event, CancellationToken cancellationToken = default)
    {
        var payment = await paymentRepository.GetByIdAsync(@event.PaymentId, cancellationToken);
        if (payment is null)
            throw new NotFoundException(nameof(Payment), @event.PaymentId);
        var paymentConfirmed = new PaymentConfirmedIntegrationEvent(payment.BookRef);
        await publisher.PublishAsync(paymentConfirmed, cancellationToken);
    }
}