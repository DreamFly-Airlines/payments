using Microsoft.Extensions.DependencyInjection;
using Payments.Application.Abstractions;

namespace Payments.Infrastructure.Events;

public class ServiceProviderEventPublisher(IServiceProvider serviceProvider) : IEventPublisher
{
    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
    {
        var handlers = serviceProvider.GetServices<IEventHandler<TEvent>>();
        foreach (var handler in handlers)
            await handler.HandleAsync(@event, cancellationToken);
    }
}