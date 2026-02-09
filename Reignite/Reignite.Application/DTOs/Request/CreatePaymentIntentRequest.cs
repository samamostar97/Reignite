using System.ComponentModel.DataAnnotations;

namespace Reignite.Application.DTOs.Request
{
    public class CreatePaymentIntentRequest
    {
        [Required(ErrorMessage = "Stavke su obavezne.")]
        [MinLength(1, ErrorMessage = "Mora imati barem jednu stavku.")]
        public List<CreateOrderItemRequest> Items { get; set; } = new();
    }
}
