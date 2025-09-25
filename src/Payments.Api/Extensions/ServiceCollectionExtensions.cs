using System.Reflection;
using Confluent.Kafka;
using Payments.Application.Abstractions;
using Payments.Application.Producers;
using Payments.Infrastructure.Producers;

namespace Payments.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddCommandHandlers(this IServiceCollection services, Assembly assembly)
    {
        var handlerInterfaceType = typeof(ICommandHandler<>);
        services.FindImplementationsAndRegister(handlerInterfaceType, assembly);
    }

    public static void AddDomainEventHandlers(this IServiceCollection services, Assembly assembly)
    {
        var handlerInterfaceType = typeof(IDomainEventHandler<>);
        services.FindImplementationsAndRegister(handlerInterfaceType, assembly);
    }

    public static void AddKafkaProducers(this IServiceCollection services, IConfigurationSection kafkaConfigSection)
    {
        var kafkaConfig = new ProducerConfig();
        kafkaConfigSection.Bind(kafkaConfig);
        services.AddSingleton<IProducer<Ignore, string>>(
            _ => new ProducerBuilder<Ignore, string>(kafkaConfig).Build());
        services.AddSingleton<IEventProducer, KafkaEventProducer>();
    }

    private static void FindImplementationsAndRegister(this IServiceCollection services, Type interfaceType, Assembly assembly)
    {
        var types = assembly
            .GetTypes()
            .Where(t => t is { IsAbstract: false, IsInterface: false })
            .Select(t => new
            {
                Implementation = t,
                Interfaces = t.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == interfaceType)
            });

        foreach (var type in types)
        foreach (var @interface in type.Interfaces)
            services.AddScoped(@interface, type.Implementation);
    }
}