using System.ComponentModel.DataAnnotations;

namespace Payments.Api.Dto;

public class MakePaymentRequest
{
    [Required] public string BookRef { get; set; } = null!;

    [Required] public string PaymentMethod { get; set; } = null!;

    [Required] public string Provider { get; set; } = null!;

    [Required] public decimal Amount { get; set; }
}