using Payments.Domain.AggregateRoots;
using Payments.Domain.Entities;
using Payments.Domain.ValueObjects;

namespace Payments.Domain.Repositories;

public interface IBillingInfoRepository
{
    public Task<BillingInfo?> GetAsync(
        string userId, 
        Channel channel, 
        CancellationToken cancellationToken = default);
    
    public Task AddAsync(BillingInfo billingInfo, CancellationToken cancellationToken = default);
    
    public Task RemoveAsync(BillingInfo billingInfo, CancellationToken cancellationToken = default);
        
    public Task SaveChangesAsync(BillingInfo billingInfo, CancellationToken cancellationToken = default);
}