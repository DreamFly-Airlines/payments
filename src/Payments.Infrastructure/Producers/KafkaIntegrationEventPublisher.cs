using System.Reflection;
using System.Text;
using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Shared.Abstractions.IntegrationEvents;
using Shared.IntegrationEvents.Kafka;

namespace Payments.Infrastructure.Producers;

public class KafkaIntegrationEventPublisher(
    ILogger<KafkaIntegrationEventPublisher> logger,
    IProducer<Null, string> producer) : IIntegrationEventPublisher
{
    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
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
            var eventNameProp = @event.GetType().GetProperty(nameof(IIntegrationEvent.EventName),
                BindingFlags.Public | BindingFlags.Static);
            var eventName = (string)eventNameProp!.GetValue(@event)!;
            logger.LogInformation(
                "Message \"{MessageValue}\" with type \"{EventType}\" formed.", message.Value, eventName);
            message.Headers.Add(KafkaHeaders.EventType, Encoding.UTF8.GetBytes(eventName));
            await producer.ProduceAsync(KafkaTopics.PaymentsEvents, message, cancellationToken);
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