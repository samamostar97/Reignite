using System.ComponentModel.DataAnnotations;

namespace Reignite.Application.DTOs.Request
{
    public class CheckoutRequest
    {
        [Required(ErrorMessage = "Stavke narudžbe su obavezne.")]
        [MinLength(1, ErrorMessage = "Narudžba mora imati barem jednu stavku.")]
        public List<CreateOrderItemRequest> Items { get; set; } = new();

        [Required(ErrorMessage = "Stripe PaymentIntent ID je obavezan.")]
        public string StripePaymentIntentId { get; set; } = string.Empty;

        public string? CouponCode { get; set; }
    }
}
