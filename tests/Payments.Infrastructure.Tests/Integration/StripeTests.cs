using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Payments.Domain.ValueObjects;
using Payments.Infrastructure.Configuration;
using Payments.Infrastructure.Services;

namespace Payments.Infrastructure.Tests.Integration;

public class StripeTests
{
    private readonly IOptions<StripeOptions> _stripeOptions;

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
        
        var options = new StripeOptions(apiKey);
        _stripeOptions = Options.Create(options);
    }
    
    [SkippableFact]
    public async Task ProcessPaymentAsync_WhenDataIsValid_ReturnsUrl()
    {
        var stripeGateway = new StripePaymentGatewayService(_stripeOptions);
        
        var url = await stripeGateway.ProcessPaymentAsync(
            "https://example.com/success", 
            Guid.NewGuid().ToString(), 
            new Money(100.00m, Currency.FromIsoString("RUB")));
        
        Assert.NotNull(url);
        Assert.StartsWith("https://checkout.stripe.com/c/pay/cs_test_", url);
    }
}