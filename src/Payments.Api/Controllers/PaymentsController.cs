using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Payments.Api.Authorization;
using Payments.Api.Dto;
using Payments.Application.Commands;
using Shared.Abstractions.Commands;

namespace Payments.Api.Controllers;

[Route("api/payments")]
public class PaymentsController(ICommandSender commandSender) : Controller
{
    [HttpGet("{paymentId}")]
    [Authorize(Policy = Policies.HasNameIdentifier)]
    public async Task<IActionResult> GetPaymentAsync([FromRoute] string paymentId)
    {
        throw new NotImplementedException();
    }
    
    [HttpPost]
    [Authorize(Policy = Policies.HasNameIdentifier)]
    public async Task<IActionResult> CreatePaymentAsync([FromBody] MakePaymentRequest makePaymentRequest)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var command = new CreatePaymentCommand(
            userId,
            makePaymentRequest.BookRef,
            makePaymentRequest.PaymentMethod, 
            makePaymentRequest.Provider,
            makePaymentRequest.Amount,
            "RUB" /* TODO: get currency from request */);
        var paymentId = await commandSender.SendAsync(command);
        return CreatedAtAction("GetPayment", new { paymentId });
    }

    [HttpPost("{paymentId}")]
    [Authorize(Policy = Policies.HasNameIdentifier)]
    public async Task<IActionResult> ProcessPaymentAsync([FromRoute] string paymentId)
    {
        var command = new ProcessPaymentCommand(paymentId);
        var returnUrl = await commandSender.SendAsync(command);
        return Ok(returnUrl);
    }
}