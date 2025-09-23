using System.ComponentModel.DataAnnotations;

namespace Payments.Api.Dto;

public class PaymentRequestDto
{
    [Required]
    public string PaymentId { get; set; }
    
    [Required]
    public string PaymentMethod { get; set; }
    
    [Required]
    public string ProviderName { get; set; }
}