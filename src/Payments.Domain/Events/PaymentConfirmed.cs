using Payments.Domain.Abstractions;

namespace Payments.Domain.Events;

public record PaymentConfirmed(string PaymentId) : IDomainEvent;