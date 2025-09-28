using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Payments.Api.Dto;
using Payments.Api.Helpers;
using Payments.Application.Abstractions;
using Payments.Application.Commands;
using Payments.Domain.Enums;

namespace Payments.Api.Controllers;

[Route("api/payments")]
public class PaymentsController(ICommandSender commandSender) : Controller
{
    [HttpPost]
    // [Authorize]
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