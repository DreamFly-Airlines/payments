using System.Security.Claims;
using Microsoft.OpenApi.Models;
using Payments.Api.Authorization;
using Payments.Api.ExceptionHandling;
using Payments.Api.Extensions;
using Payments.Application.Commands;
using Payments.Application.EventHandlers;
using Payments.Domain.Repositories;
using Payments.Infrastructure.Persistence;
using Payments.Infrastructure.Repositories;
using Shared.Abstractions.Commands;
using Shared.Abstractions.Events;
using Shared.Extensions.ServiceCollection;
using Shared.Infrastructure.Commands;
using Shared.Infrastructure.Events;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddNpgsql<PaymentsDbContext>(builder.Configuration.GetConnectionString("PaymentsDb"));
builder.Services.AddCommandHandlers(typeof(MakePaymentCommand).Assembly);
builder.Services.AddDomainEventHandlers(typeof(PaymentCreatedEventHandler).Assembly);
builder.Services.AddKafkaProducers(builder.Configuration);

builder.Services.AddScoped<IEventPublisher, ServiceProviderEventPublisher>();
builder.Services.AddScoped<ICommandSender, ServiceProviderCommandSender>();
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