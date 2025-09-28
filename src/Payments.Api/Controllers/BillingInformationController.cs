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

[Route("/api/billing-info")]
public class BillingInformationController(ICommandSender commandSender) : Controller
{
    [HttpPost]
    // [Authorize]
    public async Task<IActionResult> AddBillingInfo([FromBody] BillingInfoDto billingInfoDto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var command = new AddBillingInfoCommand(
            userId, 
            billingInfoDto.PaymentMethod, 
            billingInfoDto.Provider, 
            billingInfoDto.ProviderPaymentToken, 
            billingInfoDto.LastFour);
        await commandSender.SendAsync(command);
        return Created();
    }
}