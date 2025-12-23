using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Payments.Domain.ValueObjects;
using Payments.Infrastructure.Configuration;
using Payments.Infrastructure.Services;
using Stripe;

namespace Payments.Infrastructure.Tests.Integration;

public class StripeTests
{
    private readonly IStripeClient _stripeClient;
    
    public StripeTests()
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<StripeTests>()
            .AddEnvironmentVariables()
            .Build();
        
        var apiKey = configuration[$"{nameof(StripeOptions)}:{nameof(StripeOptions.ApiKey)}"];
        Skip.If(string.IsNullOrEmpty(apiKey),
            "Test Stripe API Key is missing in configuration");
        const string stripeTestApiKeyStart = "sk_test_";
        Skip.If(!apiKey.StartsWith(stripeTestApiKeyStart),
            $"Test Stripe API Key should start with \"{stripeTestApiKeyStart}\"");
        
        _stripeClient = new StripeClient(apiKey);
    }
    
    [SkippableFact]
    public async Task ProcessPaymentAsync_WhenDataIsValid_ReturnsUrl()
    {
        var stripeGateway = new StripePaymentGatewayService(_stripeClient);
        
        var url = await stripeGateway.ProcessPaymentAsync(
            "https://example.com/success", 
            Guid.NewGuid().ToString(), 
            new Money(100.00m, Currency.FromIsoString("RUB")));
        
        Assert.NotNull(url);
        Assert.StartsWith("https://checkout.stripe.com/c/pay/cs_test_", url);
    }
}