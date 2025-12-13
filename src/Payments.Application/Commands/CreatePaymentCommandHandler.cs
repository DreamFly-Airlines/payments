using Payments.Application.Helpers;
using Payments.Application.Services;
using Payments.Domain.AggregateRoots;
using Payments.Domain.Enums;
using Payments.Domain.Repositories;
using Payments.Domain.ValueObjects;
using Shared.Abstractions.Commands;

namespace Payments.Application.Commands;

public class CreatePaymentCommandHandler(
    IPaymentRepository paymentRepository) : ICommandHandler<CreatePaymentCommand, string>
{
    public async Task<string> HandleAsync(
        CreatePaymentCommand command, 
        CancellationToken cancellationToken = default)
    {
        var paymentId = Guid.NewGuid().ToString();
        var paymentMethod = DataParser.TryParseEnumOrThrow<PaymentMethod>(command.PaymentMethod, "payment method");
        var provider = DataParser.TryParseEnumOrThrow<Provider>(command.Provider, "provider");
        var channel = new Channel(paymentMethod, provider);
        var payment = new Payment(command.UserId, paymentId, command.BookRef, channel, command.Amount);
        await paymentRepository.AddAsync(payment.Id, payment, cancellationToken);
        return payment.Id;
    }
}