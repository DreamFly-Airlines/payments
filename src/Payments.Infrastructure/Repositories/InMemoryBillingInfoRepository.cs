using Payments.Domain.Entities;
using Payments.Domain.Repositories;
using Payments.Domain.ValueObjects;

namespace Payments.Infrastructure.Repositories;

public class InMemoryBillingInfoRepository : IBillingInfoRepository
{
    private static readonly Dictionary<(string UserId, PaymentChannel PaymentChannel), BillingInfo> 
        BillingInfos = new();
    public Task<BillingInfo?> GetAsync(
        string userId, PaymentChannel paymentChannel, CancellationToken cancellationToken = default)
    {
        var billingInfo = BillingInfos.GetValueOrDefault((userId, paymentChannel));
        return Task.FromResult(billingInfo);
    }

    public async Task AddAsync(BillingInfo billingInfo, CancellationToken cancellationToken = default)
    {
        if (!BillingInfos.TryAdd((billingInfo.UserId, billingInfo.Channel), billingInfo))
            throw new Exception();
        await SaveChangesAsync(billingInfo, cancellationToken);
    }

    public Task RemoveAsync(BillingInfo billingInfo, CancellationToken cancellationToken = default)
    {
        BillingInfos.Remove((billingInfo.UserId, billingInfo.Channel));
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(BillingInfo billingInfo, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}