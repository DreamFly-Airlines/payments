using Shared.Abstractions.Commands;

namespace Payments.Application.Commands;

public record ProcessPaymentCommand(string PaymentId) : ICommand<string>;