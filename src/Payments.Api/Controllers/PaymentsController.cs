using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Payments.Api.Dto;
using Payments.Api.Helpers;
using Payments.Application.Abstractions;
using Payments.Application.Commands;
using Payments.Domain.Enums;

namespace Payments.Api.Controllers;

[Route("api/[controller]")]
public class PaymentsController(ICommandSender commandSender) : Controller
{
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> MakePayment([FromBody] PaymentRequestDto paymentRequestDto)
    {
        if (!Enum.TryParse<PaymentMethod>(paymentRequestDto.PaymentMethod, true, out var paymentMethod))
        {
            ModelState.AddModelError(
                "Payment method", EnumFromStringValidationHelper.GetAllowedEnumValuesMessage<PaymentMethod>());
            return BadRequest(ModelState);
        }
        
        if (!Enum.TryParse<Provider>(paymentRequestDto.Provider, true, out var providerName))
        {
            ModelState.AddModelError(
                "Provider name", EnumFromStringValidationHelper.GetAllowedEnumValuesMessage<Provider>());
            return BadRequest(ModelState);
        }
        
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var command = new MakePaymentCommand(
            userId,
            paymentRequestDto.PaymentId,
            paymentMethod, 
            providerName,
            paymentRequestDto.Amount);
        await commandSender.SendAsync(command);
        return Ok();
    }
}