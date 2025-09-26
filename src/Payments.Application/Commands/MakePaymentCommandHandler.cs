using System.ComponentModel.Design;
using Payments.Application.Abstractions;
using Payments.Domain.AggregateRoots;
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
        var paymentChannel = new PaymentChannel(command.PaymentMethod, command.Provider);
        var payment = new Payment(command.UserId, command.PaymentId, paymentChannel, command.Amount);
        await paymentRepository.AddAsync(payment.PaymentId, payment, cancellationToken);
    }
}