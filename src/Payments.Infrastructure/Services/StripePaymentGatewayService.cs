using Microsoft.Extensions.Options;
using Payments.Application.Services;
using Payments.Domain.ValueObjects;
using Payments.Infrastructure.Configuration;
using Stripe;
using Stripe.Checkout;

namespace Payments.Infrastructure.Services;

public class StripePaymentGatewayService(
    IOptions<StripeOptions> options) : IPaymentGatewayService
{
    private readonly string _apiKey = options.Value.ApiKey;
        
    public async Task<string> ProcessPaymentAsync(
        string successUrl,
        string paymentId,
        Money money,
        CancellationToken cancellationToken = default)
    {
        var productData = GetProductData(paymentId);
        var priceData = GetPriceData(productData, money);
        var sessionCreateOptions = new SessionCreateOptions
        {
            SuccessUrl = successUrl,
            LineItems =
            [
                new SessionLineItemOptions
                {
                    PriceData = priceData,
                    Quantity = 1
                }
            ],
            Mode = "payment"
        };
        var checkoutService = new SessionService();
        var requestOptions = new RequestOptions { ApiKey = _apiKey };
        var session = await checkoutService.CreateAsync(
            sessionCreateOptions,
            requestOptions,
            cancellationToken: cancellationToken);
        return session.ReturnUrl;
    }

    private static SessionLineItemPriceDataProductDataOptions GetProductData(string paymentId) =>
        new()
        {
            Metadata = new() { [nameof(paymentId)] = paymentId },
            Name = "Flight Booking",
            TaxCode = "txcd_00000000"
        };

    private static SessionLineItemPriceDataOptions GetPriceData(
        SessionLineItemPriceDataProductDataOptions productData,
        Money money) =>
        new()
        {
            Currency = money.Currency.IsoCode,
            ProductData = productData,
            UnitAmount = (int)(money.Amount * (decimal)Math.Pow(10, money.Currency.MinorUnitCount))
        };
}