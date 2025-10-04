using System.Reflection;
using Confluent.Kafka;
using Payments.Application.Producers;
using Payments.Infrastructure.Producers;
using Shared.Abstractions.Commands;
using Shared.Abstractions.Events;

namespace Payments.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddKafkaProducers(this IServiceCollection services, IConfiguration configuration)
    {
        var kafkaConfig = new ProducerConfig();
        configuration.GetSection("Kafka:PaymentsEvents:ProducerSettings").Bind(kafkaConfig);
        services.AddSingleton<IProducer<Null, string>>(
            _ => new ProducerBuilder<Null, string>(kafkaConfig)
                .SetValueSerializer(Serializers.Utf8)
                .Build());
        services.AddSingleton<IIntegrationEventProducer, KafkaIntegrationEventProducer>();
    }
}