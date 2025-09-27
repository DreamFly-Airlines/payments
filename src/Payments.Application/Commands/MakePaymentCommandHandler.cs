using System.ComponentModel.Design;
using Payments.Application.Abstractions;
using Payments.Application.Exceptions;
using Payments.Application.Helpers;
using Payments.Domain.AggregateRoots;
using Payments.Domain.Enums;
using Payments.Domain.Repositories;
using Payments.Domain.ValueObjects;

namespace Payments.Application.Commands;

public class MakePaymentCommandHandler(
    IPaymentRepository paymentRepository) : ICommandHandler<MakePaymentCommand>
{
    public async Task HandleAsync(
        MakePaymentCommand command, 
        CancellationToken cancellationToken = default)
    {
        var paymentId = Guid.NewGuid().ToString();
        var paymentMethod = DataParser.TryParseEnumOrThrow<PaymentMethod>(command.PaymentMethod, "payment method");
        var provider = DataParser.TryParseEnumOrThrow<Provider>(command.Provider, "provider");
        var channel = new Channel(paymentMethod, provider);
        var payment = new Payment(command.UserId, paymentId, command.BookRef, channel, command.Amount);
        await paymentRepository.AddAsync(payment.Id, payment, cancellationToken);
    }
}