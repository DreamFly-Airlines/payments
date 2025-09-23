using Payments.Domain.ValueObjects;

namespace Payments.Domain.Entities;

public class BillingInfo(string userId, PaymentChannel channel, string providerPaymentToken, string lastFour)
{
    public string UserId = userId;
    public PaymentChannel Channel = channel;
    public string ProviderPaymentToken = providerPaymentToken;
    public string LastFour = lastFour;
}