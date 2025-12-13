using Shared.Abstractions.Commands;

namespace Payments.Application.Commands;

public record CreatePaymentCommand(
    string UserId,
    string BookRef,
    string PaymentMethod, 
    string Provider,
    decimal Amount) : ICommand<string>;