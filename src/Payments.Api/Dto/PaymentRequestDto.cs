using System.ComponentModel.DataAnnotations;

namespace Payments.Api.Dto;

public class PaymentRequestDto
{
    [Required] public string PaymentId { get; set; } = null!;

    [Required] public string PaymentMethod { get; set; } = null!;

    [Required] public string Provider { get; set; } = null!;

    [Required] public decimal Amount { get; set; }
}