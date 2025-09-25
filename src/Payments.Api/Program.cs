using Payments.Api.Extensions;
using Payments.Application.Commands;
using Payments.Application.EventHandlers;
using Payments.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddNpgsql<PaymentsDbContext>(builder.Configuration.GetConnectionString("PaymentsDb"));
builder.Services.AddCommandHandlers(typeof(MakePaymentCommand).Assembly);
builder.Services.AddDomainEventHandlers(typeof(PaymentCreatedEventHandler).Assembly);
builder.Services.AddKafkaProducers(builder.Configuration.GetSection("Kafka:ProducerSettings"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();
app.UseHttpsRedirection();

app.Run();