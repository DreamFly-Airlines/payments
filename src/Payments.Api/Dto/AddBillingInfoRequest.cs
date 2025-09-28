using System.ComponentModel.DataAnnotations;

namespace Payments.Api.Dto;

public class AddBillingInfoRequest
{
    [Required] public string PaymentMethod { get; set; } = null!;

    [Required] public string Provider { get; set; } = null!;

    [Required] public string ProviderPaymentToken { get; set; } = null!;

    [Required] public string LastFour { get; set; } = null!;
}