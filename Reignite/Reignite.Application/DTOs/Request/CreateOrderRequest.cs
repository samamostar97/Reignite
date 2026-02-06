using System.ComponentModel.DataAnnotations;

namespace Reignite.Application.DTOs.Request
{
    public class CreateOrderRequest
    {
        [Required(ErrorMessage = "Korisnik je obavezan.")]
        [Range(1, int.MaxValue, ErrorMessage = "Korisnik mora biti ispravan.")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Stavke narudžbe su obavezne.")]
        [MinLength(1, ErrorMessage = "Narudžba mora imati barem jednu stavku.")]
        public List<CreateOrderItemRequest> Items { get; set; } = new();
    }

    public class CreateOrderItemRequest
    {
        [Required(ErrorMessage = "Proizvod je obavezan.")]
        [Range(1, int.MaxValue, ErrorMessage = "Proizvod mora biti ispravan.")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Količina je obavezna.")]
        [Range(1, 100, ErrorMessage = "Količina mora biti između 1 i 100.")]
        public int Quantity { get; set; }
    }
}
