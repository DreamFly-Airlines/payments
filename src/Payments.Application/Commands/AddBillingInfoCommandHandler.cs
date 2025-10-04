using Payments.Application.Helpers;
using Payments.Domain.Entities;
using Payments.Domain.Enums;
using Payments.Domain.Repositories;
using Payments.Domain.ValueObjects;
using Shared.Abstractions.Commands;

namespace Payments.Application.Commands;

public class AddBillingInfoCommandHandler(
    IBillingInfoRepository billingInfoRepository) : ICommandHandler<AddBillingInfoCommand>
{
    public async Task HandleAsync(AddBillingInfoCommand command, CancellationToken cancellationToken = default)
    {
        var lastFour = LastFour.FromString(command.LastFour);
        var paymentMethod = DataParser.TryParseEnumOrThrow<PaymentMethod>(command.PaymentMethod, "payment method");
        var provider = DataParser.TryParseEnumOrThrow<Provider>(command.Provider, "provider");
        var channel = new Channel(paymentMethod, provider);
        var billingInfo = new BillingInfo(
            command.UserId, channel, command.ProviderPaymentToken, lastFour);
        await  billingInfoRepository.AddAsync(billingInfo, cancellationToken);
    }
}