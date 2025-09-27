using Payments.Domain.Enums;

namespace Payments.Domain.ValueObjects;

public readonly record struct Channel(
    PaymentMethod PaymentMethod,
    Provider Provider);