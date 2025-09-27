using Payments.Application.Abstractions;
using Payments.Domain.Enums;

namespace Payments.Application.Commands;

public record MakePaymentCommand(
    string UserId,
    string BookRef,
    string PaymentMethod, 
    string Provider,
    decimal Amount) : ICommand;