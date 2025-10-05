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
    [HttpPost]
    [Authorize(Policy = Policies.HasNameIdentifier)]
    public async Task<IActionResult> MakePayment([FromBody] MakePaymentRequest makePaymentRequest)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var command = new MakePaymentCommand(
            userId,
            makePaymentRequest.BookRef,
            makePaymentRequest.PaymentMethod, 
            makePaymentRequest.Provider,
            makePaymentRequest.Amount);
        await commandSender.SendAsync(command);
        return Created();
    }
}