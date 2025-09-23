using Payments.Application.Abstractions;
using Payments.Domain.Enums;

namespace Payments.Application.Commands;

public record MakePaymentCommand(
    string UserId, 
    string PaymentId, 
    PaymentMethod PaymentMethod, 
    ProviderName Provider) : ICommand;