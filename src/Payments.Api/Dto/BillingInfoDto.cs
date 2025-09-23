using System.ComponentModel.DataAnnotations;

namespace Payments.Api.Dto;

public class BillingInfoDto
{
    [Required]
    public string PaymentMethod { get; set; }
    
    [Required]
    public string Provider { get; set; }
    
    [Required]
    public string ProviderPaymentToken { get; set; }
    
    [Required]
    public string LastFour { get; set; }
}