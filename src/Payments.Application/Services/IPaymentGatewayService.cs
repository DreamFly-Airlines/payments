namespace Payments.Application.Services;

public interface IPaymentGatewayService
{
    public Task<string> ProcessPaymentAsync(CancellationToken cancellationToken = default);
}