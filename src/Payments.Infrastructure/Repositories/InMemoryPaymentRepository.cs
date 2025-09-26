using Microsoft.Extensions.DependencyInjection;
using Payments.Application.Abstractions;
using Payments.Domain.AggregateRoots;
using Payments.Domain.Repositories;

namespace Payments.Infrastructure.Repositories;

public class InMemoryPaymentRepository(IServiceScopeFactory scopeFactory) : IPaymentRepository
{
    private static readonly Dictionary<string, Payment> Payments = new();
    public Task<Payment?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var payment = Payments.GetValueOrDefault(id);
        return Task.FromResult(payment);
    }

    public async Task AddAsync(string id, Payment payment, CancellationToken cancellationToken = default)
    {
        if (!Payments.TryAdd(id, payment))
            throw new Exception();
        await SaveChangesAsync(payment, cancellationToken);
    }

    public Task RemoveAsync(Payment payment, CancellationToken cancellationToken = default)
    {
        Payments.Remove(payment.PaymentId);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync(Payment payment, CancellationToken cancellationToken = default)
    {
        using var scope = scopeFactory.CreateScope();
        var eventPublisher = scope.ServiceProvider.GetRequiredService<IEventPublisher>();
        var events = payment.DomainEvents.ToArray();
        payment.ClearDomainEvents();
        foreach (var @event in events)
            await eventPublisher.PublishAsync(@event, cancellationToken);
    }
}