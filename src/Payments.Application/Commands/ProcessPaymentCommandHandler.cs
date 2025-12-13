using Payments.Application.Services;
using Payments.Domain.Repositories;
using Shared.Abstractions.Commands;

namespace Payments.Application.Commands;

public class ProcessPaymentCommandHandler(
    IPaymentRepository paymentRepository,
    IPaymentGatewayService paymentGatewayService) : ICommandHandler<ProcessPaymentCommand, string>
{
    public async Task<string> HandleAsync(ProcessPaymentCommand command, CancellationToken cancellationToken = default)
    {
        var payment = await paymentRepository.GetByIdAsync(command.PaymentId, cancellationToken);
        var returnUrl = await paymentGatewayService.ProcessPaymentAsync(cancellationToken);
        return returnUrl;
    }
}