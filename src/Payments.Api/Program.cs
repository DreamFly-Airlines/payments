using Payments.Api.ExceptionHandling;
using Payments.Api.Extensions;
using Payments.Application.Commands;
using Payments.Application.EventHandlers;
using Payments.Domain.Repositories;
using Payments.Infrastructure.Configuration;
using Payments.Infrastructure.Persistence;
using Payments.Infrastructure.Producers;
using Payments.Infrastructure.Repositories;
using Shared.Abstractions.Commands;
using Shared.Abstractions.Events;
using Shared.Abstractions.IntegrationEvents;
using Shared.Extensions.ServiceCollection;
using Shared.Infrastructure.Commands;
using Shared.Infrastructure.Events;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<StripeOptions>(builder.Configuration.GetSection(nameof(StripeOptions)));

builder.Services.AddControllers();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddSingleton<IStripeClient>(_ => new StripeClient(builder.Configuration["Stripe:ApiKey"]));
builder.Services.AddNpgsql<PaymentsDbContext>(builder.Configuration.GetConnectionString("PaymentsDb"));
builder.Services.AddCommandHandlers(typeof(CreatePaymentCommand).Assembly);
builder.Services.AddDomainEventHandlers(typeof(PaymentConfirmedEventHandler).Assembly);
builder.Services.AddKafkaProducers(builder.Configuration);

builder.Services.AddScoped<IEventPublisher, ServiceProviderEventPublisher>();
builder.Services.AddScoped<ICommandSender, ServiceProviderCommandSender>();
builder.Services.AddSingleton<IIntegrationEventPublisher, KafkaIntegrationEventPublisher>();
builder.Services.AddSingleton<IPaymentRepository, InMemoryPaymentRepository>();
builder.Services.AddSingleton<IBillingInfoRepository, InMemoryBillingInfoRepository>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGenWithJwtAuthentication();

builder.Services.AddAuthenticationWithJwt(builder.Configuration);
builder.Services.AddAuthorizationWithPolicies();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();