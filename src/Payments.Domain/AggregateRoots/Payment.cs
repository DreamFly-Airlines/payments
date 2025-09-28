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

    public void MarkAsConfirmed()
    {
        if (TryChangeStatusFromPendingAndPublish(Status.Confirmed)) 
            return;
        
        var reason = Status switch
        {
            Status.Cancelled => "it's cancelled",
            Status.Confirmed => "it's already confirmed",
            _ => throw new ArgumentException($"Unexpected status: {Status}")
        };
        throw new InvalidDomainOperationException($"Cannot confirm payment because {reason}.");
    }

    public void Cancel() 
    {
        if (TryChangeStatusFromPendingAndPublish(Status.Cancelled)) 
            return;
        
        var reason = Status switch
        {
            Status.Cancelled => "it's already cancelled",
            Status.Confirmed => "it's confirmed",
            _ => throw new ArgumentException($"Unexpected status: {Status}")
        };
        throw new InvalidDomainOperationException($"Cannot confirm payment because {reason}.");
    }

    private bool TryChangeStatusFromPendingAndPublish(Status status)
    {
        if (Status is Status.Pending)
        {
            Status = status;
            AddDomainEvent(Status is Status.Confirmed
                ? new PaymentConfirmed(Id)
                : new PaymentCancelled(Id));
            return true;
        }

        return false;
    }
}