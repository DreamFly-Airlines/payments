using Payments.Domain.Abstractions;
using Payments.Domain.Enums;
using Payments.Domain.Events;
using Payments.Domain.Exceptions;
using Payments.Domain.ValueObjects;

namespace Payments.Domain.AggregateRoots;

public class Payment : AggregateRoot<IDomainEvent>
{
    public string Id { get; }
    public string UserId { get; }
    public string BookRef { get; }
    public Status Status { get; private set; }
    public Channel Channel { get; }
    public decimal Amount { get; }

    public Payment(string userId, string id, string bookRef, Channel channel,  decimal amount)
    {
        UserId = userId;
        Id = id;
        BookRef = bookRef;
        Status = Status.Pending;
        Channel = channel;
        Amount = amount;
        AddDomainEvent(new PaymentCreated(Id));
    }
    
    public void MarkAsConfirmed() => MarkAsConfirmedOrCancelAndPublish(Status.Confirmed);

    public void Cancel() => MarkAsConfirmedOrCancelAndPublish(Status.Cancelled);

    private void MarkAsConfirmedOrCancelAndPublish(Status status)
    {
        if (Status is Status.Pending)
        {
            Status = status;
            AddDomainEvent(Status is Status.Confirmed
                ? new PaymentConfirmed(Id, BookRef)
                : new PaymentCancelled(Id, BookRef));
        }
        else
            throw new InvalidDomainOperationException(
                $"Cannot set {nameof(Status)} of {nameof(Payment)} " +
                $"with ID \"{Id}\" to {status} " +
                $"because {nameof(Status)} is not {nameof(Status.Pending)}, it is {Status}.");
    }
}