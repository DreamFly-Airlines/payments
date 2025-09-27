using Payments.Domain.Abstractions;

namespace Payments.Domain.Events;

public record PaymentCancelled(string PaymentId, string BookRef) : IDomainEvent;