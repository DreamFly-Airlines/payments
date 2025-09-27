using Payments.Application.Abstractions;
using Payments.Application.Exceptions;
using Payments.Domain.AggregateRoots;
using Payments.Domain.Events;
using Payments.Domain.Exceptions;
using Payments.Domain.Repositories;

namespace Payments.Application.EventHandlers;

public class PaymentCreatedEventHandler(
    IPaymentRepository paymentRepository) : IEventHandler<PaymentCreated>
{
    public async Task HandleAsync(PaymentCreated @event, CancellationToken cancellationToken = default)
    {
        var payment = await paymentRepository.GetByIdAsync(@event.PaymentId, cancellationToken);
        if (payment == null)
            throw new NotFoundException(nameof(Payment), @event.PaymentId);
        // TODO: initiate a payment via the provider’s API
        await Task.Delay(500);
        try
        {
            payment.MarkAsConfirmed();
            await paymentRepository.SaveChangesAsync(payment, cancellationToken);
        }
        catch (InvalidDomainOperationException ex)
        {
            throw new ValidationException(ex.Message);
        }
    }
}