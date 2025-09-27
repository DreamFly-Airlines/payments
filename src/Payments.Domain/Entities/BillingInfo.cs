using Payments.Domain.ValueObjects;

namespace Payments.Domain.Entities;

public class BillingInfo
{
    public string UserId { get; }
    public Channel Channel { get; }
    public string ProviderPaymentToken { get; }
    public LastFour LastFour { get; }

    public BillingInfo(string userId, Channel channel, string providerPaymentToken, LastFour lastFour)
    {
        UserId = userId;
        Channel = channel;
        ProviderPaymentToken = providerPaymentToken;
        LastFour = lastFour;
    }
}