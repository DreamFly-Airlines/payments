using Payments.Application.Abstractions;
using Payments.Domain.AggregateRoots;
using Payments.Domain.Repositories;

namespace Payments.Infrastructure.Repositories;

public class InMemoryPaymentRepository(IEventPublisher eventPublisher) : IPaymentRepository
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
        payment.ClearDomainEvents();
    }

    public Task RemoveAsync(Payment payment, CancellationToken cancellationToken = default)
    {
        Payments.Remove(payment.PaymentId);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync(Payment payment, CancellationToken cancellationToken = default)
    {
        foreach (var @event in payment.DomainEvents)
            await eventPublisher.PublishAsync(@event, cancellationToken);
        payment.ClearDomainEvents();
    }
}