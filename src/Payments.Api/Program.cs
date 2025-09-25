using Payments.Api.Extensions;
using Payments.Application.Abstractions;
using Payments.Application.Commands;
using Payments.Application.EventHandlers;
using Payments.Domain.Repositories;
using Payments.Infrastructure.Events;
using Payments.Infrastructure.Persistence;
using Payments.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddNpgsql<PaymentsDbContext>(builder.Configuration.GetConnectionString("PaymentsDb"));
builder.Services.AddCommandHandlers(typeof(MakePaymentCommand).Assembly);
builder.Services.AddDomainEventHandlers(typeof(PaymentCreatedEventHandler).Assembly);
builder.Services.AddKafkaProducers(builder.Configuration);
builder.Services.AddScoped<IEventPublisher, ServiceProviderEventPublisher>();
builder.Services.AddSingleton<IPaymentRepository, InMemoryPaymentRepository>();
builder.Services.AddSingleton<IBillingInfoRepository, InMemoryBillingInfoRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();
app.UseHttpsRedirection();

app.Run();