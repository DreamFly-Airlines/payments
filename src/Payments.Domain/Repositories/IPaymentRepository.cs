using Payments.Domain.Abstractions;
using Payments.Domain.AggregateRoots;

namespace Payments.Domain.Repositories;

public interface IPaymentRepository
{
    public Task<Payment?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    
    public Task AddAsync(string id, Payment payment, CancellationToken cancellationToken = default);
        
    public Task RemoveAsync(Payment payment, CancellationToken cancellationToken = default);
        
    public Task SaveChangesAsync(Payment payment, CancellationToken cancellationToken = default);
}