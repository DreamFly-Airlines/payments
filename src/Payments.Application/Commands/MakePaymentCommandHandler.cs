using Payments.Application.Abstractions;
using Payments.Domain.AggregateRoots;
using Payments.Domain.ValueObjects;

namespace Payments.Application.Commands;

public class MakePaymentCommandHandler : ICommandHandler<MakePaymentCommand>
{
    public async Task HandleAsync(
        MakePaymentCommand command, 
        CancellationToken cancellationToken = default)
    {
        var paymentChannel = new PaymentChannel(command.PaymentMethod, command.Provider);
        var payment = new Payment(command.UserId, command.PaymentId, paymentChannel);
    }
}