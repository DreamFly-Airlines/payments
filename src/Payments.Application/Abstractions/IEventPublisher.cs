namespace Payments.Application.Abstractions;

public interface IEventPublisher
{
    public Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default);
}