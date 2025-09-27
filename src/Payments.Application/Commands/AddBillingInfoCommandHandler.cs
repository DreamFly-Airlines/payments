using Payments.Application.Abstractions;
using Payments.Domain.Entities;
using Payments.Domain.Repositories;
using Payments.Domain.ValueObjects;

namespace Payments.Application.Commands;

public class AddBillingInfoCommandHandler(
    IBillingInfoRepository billingInfoRepository) : ICommandHandler<AddBillingInfoCommand>
{
    public async Task HandleAsync(AddBillingInfoCommand command, CancellationToken cancellationToken = default)
    {
        var lastFour = LastFour.FromString(command.LastFour);
        var billingInfo = new BillingInfo(
            command.UserId, command.Channel, command.ProviderPaymentToken, lastFour);
        await  billingInfoRepository.AddAsync(billingInfo, cancellationToken);
    }
}