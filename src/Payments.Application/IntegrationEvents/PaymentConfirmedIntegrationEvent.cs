using Payments.Application.Abstractions;

namespace Payments.Application.IntegrationEvents;

public class PaymentConfirmedIntegrationEvent : IIntegrationEvent
{
    public required string BookRef { get; init; }
}