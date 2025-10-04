using ICommand = Shared.Abstractions.Commands.ICommand;

namespace Payments.Application.Commands;

public record AddBillingInfoCommand(
    string UserId, 
    string PaymentMethod,
    string Provider,
    string ProviderPaymentToken, 
    string LastFour) : ICommand;