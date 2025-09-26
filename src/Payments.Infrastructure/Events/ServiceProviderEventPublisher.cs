using Microsoft.Extensions.DependencyInjection;
using Payments.Application.Abstractions;
using Payments.Domain.Abstractions;

namespace Payments.Infrastructure.Events;

public class ServiceProviderEventPublisher(IServiceProvider serviceProvider) : IEventPublisher
{
    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) 
        where TEvent : notnull
    {
        var handlerType = typeof(IEventHandler<>).MakeGenericType(@event.GetType());
        var handlers = serviceProvider.GetServices(handlerType);
        foreach (var handler in handlers)
        {
            var handleMethod = handlerType.GetMethod(nameof(IEventHandler<object>.HandleAsync))
                               ?? throw new ArgumentNullException(nameof(IEventHandler<object>.HandleAsync));
            await (Task)handleMethod.Invoke(handler, [@event, cancellationToken])!;
        }
    }
}