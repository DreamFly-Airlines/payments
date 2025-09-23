using Payments.Application.Abstractions;
using Payments.Domain.ValueObjects;

namespace Payments.Application.Commands;

public record AddBillingInfoCommand(
    string UserId, 
    PaymentChannel Channel, 
    string ProviderPaymentToken, 
    string LastFour) : ICommand;