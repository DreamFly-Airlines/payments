using Payments.Application.Abstractions;
using Payments.Application.Exceptions;
using Payments.Domain.AggregateRoots;
using Payments.Domain.Events;
using Payments.Domain.Repositories;

namespace Payments.Application.EventHandlers;

public class PaymentCreatedEventHandler(
    IPaymentRepository paymentRepository) : IEventHandler<PaymentCreated>
{
    public async Task HandleAsync(PaymentCreated domainEvent, CancellationToken cancellationToken = default)
    {
        var payment = await paymentRepository.GetByIdAsync(domainEvent.PaymentId, cancellationToken);
        if (payment == null)
            throw new NotFoundException(nameof(Payment), domainEvent.PaymentId);
        // TODO: initiate a payment via the provider’s API
        await Task.Delay(500);
        payment.MarkAsConfirmed();
        await paymentRepository.SaveChangesAsync(payment, cancellationToken);
    }
}