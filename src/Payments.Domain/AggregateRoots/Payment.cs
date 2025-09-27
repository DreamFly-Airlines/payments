using Payments.Domain.Abstractions;
using Payments.Domain.Enums;
using Payments.Domain.Events;
using Payments.Domain.Exceptions;
using Payments.Domain.ValueObjects;

namespace Payments.Domain.AggregateRoots;

public class Payment : AggregateRoot<IDomainEvent>
{
    public string UserId { get; }
    public string PaymentId { get; }
    public string BookRef { get; }
    public Status Status { get; private set; }
    public PaymentChannel Channel { get; }
    public decimal Amount { get; }

    public Payment(string userId, string paymentId, string bookRef, PaymentChannel paymentChannel,  decimal amount)
    {
        UserId = userId;
        PaymentId = paymentId;
        BookRef = bookRef;
        Status = Status.Pending;
        Channel = paymentChannel;
        Amount = amount;
        AddDomainEvent(new PaymentCreated(PaymentId));
    }
    
    public void MarkAsConfirmed() => MarkAsConfirmedOrCancelAndPublish(Status.Confirmed);

    public void Cancel() => MarkAsConfirmedOrCancelAndPublish(Status.Cancelled);

    private void MarkAsConfirmedOrCancelAndPublish(Status status)
    {
        if (Status is Status.Pending)
        {
            Status = status;
            AddDomainEvent(Status is Status.Confirmed
                ? new PaymentConfirmed(PaymentId, BookRef)
                : new PaymentCancelled(PaymentId, BookRef));
        }
        else
            throw new InvalidDomainOperationException(
                $"Cannot set {nameof(Status)} of {nameof(Payment)} " +
                $"with ID \"{PaymentId}\" to {status} " +
                $"because {nameof(Status)} is not {nameof(Status.Pending)}, it is {Status}.");
    }
}