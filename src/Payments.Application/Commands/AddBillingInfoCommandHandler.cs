using Payments.Application.Abstractions;
using Payments.Domain.Entities;
using Payments.Domain.Repositories;

namespace Payments.Application.Commands;

public class AddBillingInfoCommandHandler(
    IBillingInfoRepository billingInfoRepository) : ICommandHandler<AddBillingInfoCommand>
{
    public async Task HandleAsync(AddBillingInfoCommand command, CancellationToken cancellationToken = default)
    {
        var billingInfo = new BillingInfo(
            command.UserId, command.Channel, command.ProviderPaymentToken, command.LastFour);
        await  billingInfoRepository.AddAsync(billingInfo, cancellationToken);
    }
}