using Payments.Domain.ValueObjects;

namespace Payments.Application.Services;

public interface IPaymentGatewayService
{
    public Task<string> ProcessPaymentAsync(
        string successUrl,
        string paymentId,
        Money money,
        CancellationToken cancellationToken = default);
}