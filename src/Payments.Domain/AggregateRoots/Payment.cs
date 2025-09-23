using Payments.Domain.Abstractions;
using Payments.Domain.Enums;
using Payments.Domain.Events;
using Payments.Domain.Exceptions;

namespace Payments.Domain.AggregateRoots;

public class Payment : AggregateRoot<IDomainEvent>
{
    public string PaymentId { get; }
    public PaymentStatus Status { get; private set; }

    public Payment(string paymentId)
    {
        PaymentId = paymentId;
        Status = PaymentStatus.Pending;
        AddDomainEvent(new PaymentCreated(PaymentId));
    }
    
    public void MarkAsConfirmed() => MarkAsConfirmedOrCancelAndPublish(PaymentStatus.Confirmed);

    public void Cancel() => MarkAsConfirmedOrCancelAndPublish(PaymentStatus.Cancelled);

    private void MarkAsConfirmedOrCancelAndPublish(PaymentStatus status)
    {
        if (Status is PaymentStatus.Pending)
        {
            Status = status;
            AddDomainEvent(Status is PaymentStatus.Confirmed
                ? new PaymentConfirmed(PaymentId)
                : new PaymentCancelled(PaymentId));
        }
        else
            throw new InvalidDomainOperationException(
                $"Cannot set {nameof(Status)} of {nameof(Payment)} " +
                $"with ID \"{PaymentId}\" to {status} " +
                $"because {nameof(Status)} is not {nameof(PaymentStatus.Pending)}, it is {Status}.");
    }
}