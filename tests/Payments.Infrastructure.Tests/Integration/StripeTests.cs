using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Payments.Domain.ValueObjects;
using Payments.Infrastructure.Services;
using Stripe;

namespace Payments.Infrastructure.Tests.Integration;

public class StripeTests : IClassFixture<StripeTests.StripeContainerFixture>
{
    private readonly IStripeClient _stripeClient;
    
    public StripeTests(StripeContainerFixture fixture)
    {
        const string apiKey = "sk_test_mock";
        _stripeClient = new StripeClient(
            apiKey: apiKey,
            apiBase: fixture.ApiBaseUrl);
    }
    
    [Fact]
    public async Task ProcessPaymentAsync_WhenDataIsValid_ReturnsUrl()
    {
        var stripeGateway = new StripePaymentGatewayService(_stripeClient);
        
        var url = await stripeGateway.ProcessPaymentAsync(
            "https://example.com/success", 
            Guid.NewGuid().ToString(), 
            new Money(100.00m, Currency.FromIsoString("RUB")));
        
        Assert.NotNull(url);
        Assert.StartsWith("https://checkout.stripe.com", url);
    }
    
    public class StripeContainerFixture : IAsyncLifetime
    {
        private readonly IContainer _stripeContainer;
        private const int InternalContainerPort = 12111;

        public string ApiBaseUrl =>
            $"http://{_stripeContainer.Hostname}:{_stripeContainer.GetMappedPublicPort(InternalContainerPort)}";


        public StripeContainerFixture()
        {
            _stripeContainer = new ContainerBuilder()
                .WithImage("stripe/stripe-mock:v0.197.0")
                .WithPortBinding(InternalContainerPort, true)
                .WithWaitStrategy(Wait
                    .ForUnixContainer()
                    .UntilInternalTcpPortIsAvailable(InternalContainerPort))
                .Build();
        }
    
        public async Task InitializeAsync() => await _stripeContainer.StartAsync();

        public async Task DisposeAsync() => await _stripeContainer.StopAsync();
    }
}