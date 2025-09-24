using System.Reflection;
using Payments.Application.Abstractions;

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