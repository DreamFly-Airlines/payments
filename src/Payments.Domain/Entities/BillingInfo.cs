using Payments.Domain.ValueObjects;

namespace Payments.Domain.Entities;

public class BillingInfo(string userId, PaymentChannel channel, string lastFour)
{
    public string UserId = userId;
    public PaymentChannel Channel = channel;
    public string LastFour = lastFour;
}