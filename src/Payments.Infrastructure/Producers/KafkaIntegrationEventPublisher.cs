using System.Collections.Concurrent;
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
    private static readonly ConcurrentDictionary<Type, byte[]> EventNameCache = new();
    
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
            var eventName = GetEventNameOrThrow(@event);
            message.Headers.Add(KafkaHeaders.EventType, eventName);
            await producer.ProduceAsync(KafkaTopics.PaymentsEvents, message, cancellationToken);
            
            logger.LogInformation(
                "Message \"{MessageValue}\" with type \"{EventType}\" produced", message.Value, eventName);
        }
        catch (ProduceException<Null, string> e)
        {
            logger.LogCritical("{ErrorMessage}", e.Message);
        }
        catch (Exception e)
        {
            logger.LogCritical("{ErrorMessage}", e.Message);
        }
    }

    private static byte[] GetEventNameOrThrow<TEvent>(TEvent @event) where TEvent : IIntegrationEvent
    {
        var eventType = @event.GetType();
        const string eventNamePropName = nameof(IIntegrationEvent.EventName);
        return EventNameCache.GetOrAdd(eventType, static type =>
        {
            var prop = type.GetProperty(eventNamePropName, BindingFlags.Public | BindingFlags.Static)
                       ?? throw new MissingMemberException(
                           $"Property \"{eventNamePropName}\" is missing on type {type.FullName}");

            var value = (string?)prop.GetValue(null);
            if (string.IsNullOrEmpty(value))
                throw new MissingMemberException(
                    $"Value for property \"{eventNamePropName}\" is missing on type {type.FullName}");

            return Encoding.UTF8.GetBytes(value);
        });
    }
}