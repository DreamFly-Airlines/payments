using Shared.Abstractions.IntegrationEvents;

namespace Payments.Application.IntegrationEvents;

public class PaymentCancelledIntegrationEvent : IIntegrationEvent
{
    public required string BookRef { get; init; }
}