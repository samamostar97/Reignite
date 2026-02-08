using System.ComponentModel.DataAnnotations;

namespace Reignite.Application.DTOs.Request
{
    public class CheckoutRequest
    {
        [Required(ErrorMessage = "Stavke narudžbe su obavezne.")]
        [MinLength(1, ErrorMessage = "Narudžba mora imati barem jednu stavku.")]
        public List<CreateOrderItemRequest> Items { get; set; } = new();
    }
}
