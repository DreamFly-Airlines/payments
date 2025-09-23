using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Payments.Api.Dto;
using Payments.Application.Abstractions;
using Payments.Application.Commands;
using Payments.Domain.Enums;

namespace Payments.Api.Controllers;

[Route("api/[controller]")]
public class PaymentsController(ICommandSender commandSender) : Controller
{
    private static readonly Dictionary<Type, HashSet<string>> AllowedEnumValuesCache = new();
    
    [HttpPost("")]
    [Authorize]
    public async Task<IActionResult> MakePayment([FromBody] PaymentRequestDto paymentRequestDto)
    {
        if (!Enum.TryParse<PaymentMethod>(paymentRequestDto.PaymentMethod, true, out var paymentMethod))
        {
            ModelState.AddModelError("Payment method", GetAllowedEnumValuesMessage<PaymentMethod>());
            return BadRequest(ModelState);
        }
        
        if (!Enum.TryParse<ProviderName>(paymentRequestDto.ProviderName, true, out var providerName))
        {
            ModelState.AddModelError("Provider name", GetAllowedEnumValuesMessage<ProviderName>());
            return BadRequest(ModelState);
        }
        
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var command = new MakePaymentCommand(
            userId,
            paymentRequestDto.PaymentId,
            paymentMethod, 
            providerName);
        await commandSender.SendAsync(command);
        return Ok();
    }
    
    private static HashSet<string> GetAllowedEnumValues<T>() where T : Enum
    {
        if (AllowedEnumValuesCache.TryGetValue(typeof(T), out var allowed))
            return allowed;
        var result = new HashSet<string>();
        foreach (var name in Enum.GetNames(typeof(T)))
            result.Add(name.ToLower());
        AllowedEnumValuesCache[typeof(T)] = result;
        return result;
    }

    private static string GetAllowedEnumValuesMessage<T>() where T : Enum
    {
        var allowed = GetAllowedEnumValues<T>();
        return $"Cannot convert {typeof(T).Name}, allowed values: {string.Join(", ", allowed)}.";
    }
}