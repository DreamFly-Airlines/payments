using Payments.Domain.Abstractions;

namespace Payments.Application.Abstractions;

public interface IEventHandler<in TEvent>
{
    public Task HandleAsync(TEvent domainEvent, CancellationToken cancellationToken = default);
}