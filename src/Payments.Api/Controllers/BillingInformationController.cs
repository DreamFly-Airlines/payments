using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Payments.Api.Authorization;
using Payments.Api.Dto;
using Payments.Application.Commands;
using Shared.Abstractions.Commands;

namespace Payments.Api.Controllers;

[Route("/api/billing-info")]
public class BillingInformationController(ICommandSender commandSender) : Controller
{
    [HttpPost]
    [Authorize(Policy = Policies.HasNameIdentifier)]
    public async Task<IActionResult> AddBillingInfo([FromBody] AddBillingInfoRequest addBillingInfoRequest)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var command = new AddBillingInfoCommand(
            userId, 
            addBillingInfoRequest.PaymentMethod, 
            addBillingInfoRequest.Provider, 
            addBillingInfoRequest.ProviderPaymentToken, 
            addBillingInfoRequest.LastFour);
        await commandSender.SendAsync(command);
        return Created();
    }
}