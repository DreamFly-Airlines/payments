using Payments.Domain.Abstractions;

namespace Payments.Domain.Events;

public record PaymentCreated(string PaymentId) : IDomainEvent;