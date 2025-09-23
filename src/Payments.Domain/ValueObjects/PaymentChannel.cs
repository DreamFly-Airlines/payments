using Payments.Domain.Enums;

namespace Payments.Domain.ValueObjects;

public readonly record struct PaymentChannel(
    PaymentMethod PaymentMethod,
    ProviderName ProviderName);