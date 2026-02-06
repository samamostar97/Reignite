using System.ComponentModel.DataAnnotations;

namespace Reignite.Application.DTOs.Request
{
    public class CreateProductReviewRequest
    {
        [Required(ErrorMessage = "Proizvod je obavezan.")]
        [Range(1, int.MaxValue, ErrorMessage = "Proizvod mora biti ispravan.")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Ocjena je obavezna.")]
        [Range(1, 5, ErrorMessage = "Ocjena mora biti između 1 i 5.")]
        public int Rating { get; set; }

        [StringLength(1000, ErrorMessage = "Komentar ne smije biti duži od 1000 znakova.")]
        public string? Comment { get; set; }
    }
}
