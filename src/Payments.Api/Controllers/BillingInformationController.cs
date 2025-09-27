using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Payments.Api.Dto;
using Payments.Api.Helpers;
using Payments.Application.Abstractions;
using Payments.Application.Commands;
using Payments.Domain.Enums;
using Payments.Domain.ValueObjects;

namespace Payments.Api.Controllers;

[Route("/api/[controller]")]
public class BillingInformationController(ICommandSender commandSender) : Controller
{
    [HttpPost]
    // [Authorize]
    public async Task<IActionResult> AddBillingInfo([FromBody] BillingInfoDto billingInfoDto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        if (!Enum.TryParse<PaymentMethod>(billingInfoDto.PaymentMethod, true, out var paymentMethod))
        {
            ModelState.AddModelError(
                "Payment method", EnumFromStringValidationHelper.GetAllowedEnumValuesMessage<PaymentMethod>());
            return BadRequest(ModelState);
        }
        
        if (!Enum.TryParse<Provider>(billingInfoDto.Provider, true, out var providerName))
        {
            ModelState.AddModelError(
                "Provider name", EnumFromStringValidationHelper.GetAllowedEnumValuesMessage<Provider>());
            return BadRequest(ModelState);
        }

        var channel = new Channel(paymentMethod, providerName);
        var command = new AddBillingInfoCommand(
            userId, channel, billingInfoDto.ProviderPaymentToken, billingInfoDto.LastFour);
        await commandSender.SendAsync(command);
        return Ok();
    }
}