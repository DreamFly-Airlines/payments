using Payments.Application.Abstractions;
using Payments.Domain.ValueObjects;

namespace Payments.Application.Commands;

public record AddBillingInfoCommand(
    string UserId, 
    string PaymentMethod,
    string Provider,
    string ProviderPaymentToken, 
    string LastFour) : ICommand;