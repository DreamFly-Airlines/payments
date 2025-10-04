using Payments.Application.Exceptions;
using Payments.Domain.AggregateRoots;
using Payments.Domain.Events;
using Payments.Domain.Exceptions;
using Payments.Domain.Repositories;
using Shared.Abstractions.Events;

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
            var state = new EntityStateInfo(nameof(Payment), 
                (nameof(Payment.Id), payment.Id), 
                (nameof(Payment.Status), payment.Status.ToString()));
            throw new ValidationException(ex.Message, state);
        }
    }
}