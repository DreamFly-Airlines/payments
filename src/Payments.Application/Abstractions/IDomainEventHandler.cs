using Payments.Domain.Abstractions;

namespace Payments.Application.Abstractions;

public interface IDomainEventHandler<in TEvent> where TEvent : IDomainEvent
{
    public Task HandleAsync(TEvent domainEvent, CancellationToken cancellationToken = default);
}