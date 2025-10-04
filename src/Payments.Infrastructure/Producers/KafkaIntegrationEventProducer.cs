using System.Text;
using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Payments.Application.IntegrationEvents;
using Payments.Domain.Events;
using Shared.Abstractions.IntegrationEvents;
using IIntegrationEventProducer = Payments.Application.Producers.IIntegrationEventProducer;

namespace Payments.Infrastructure.Producers;

public class KafkaIntegrationEventProducer(
    ILogger<KafkaIntegrationEventProducer> logger,
    IProducer<Null, string> producer) : IIntegrationEventProducer
{
    private const string PaymentsEventsTopicName = "payments-events";
    private const string EventTypeHeaderName = "event-type";
    private const string PaymentConfirmedEventName = "PaymentConfirmed";
    private const string PaymentCancelledEventName = "PaymentCancelled";
    
    public async Task ProduceAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IIntegrationEvent
    {
        try
        {
            var messageJson = JsonSerializer.Serialize(@event);
            var message = new Message<Null, string>
            {
                Headers = [],
                Value = messageJson
            };
            var eventType = @event switch
            {
                PaymentConfirmedIntegrationEvent => PaymentConfirmedEventName,
                PaymentCancelledIntegrationEvent => PaymentCancelledEventName,
                _ => throw new ArgumentException(
                    $"Unknown event type \"{@event.GetType()}\". " +
                    $"Supported event types: {string.Join(", ", nameof(PaymentConfirmed), nameof(PaymentCancelled))}")
            };
            logger.LogInformation(
                "Message \"{MessageValue}\" with type \"{EventType}\" formed.", message.Value, eventType);
            message.Headers.Add(EventTypeHeaderName, Encoding.UTF8.GetBytes(eventType));
            await producer.ProduceAsync(PaymentsEventsTopicName, message, cancellationToken);
        }
        catch (ProduceException<Null, string> e)
        {
            logger.LogCritical("{ErrorMessage}", e.Message);
        }
        catch (Exception e)
        {
            logger.LogError("{ErrorMessage}", e.Message);
        }
    }
}