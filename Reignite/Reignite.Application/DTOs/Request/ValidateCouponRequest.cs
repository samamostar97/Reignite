using System.ComponentModel.DataAnnotations;

namespace Reignite.Application.DTOs.Request
{
    public class ValidateCouponRequest
    {
        [Required(ErrorMessage = "Kod kupona je obavezan.")]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ukupan iznos narudžbe je obavezan.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Iznos mora biti veći od 0.")]
        public decimal OrderTotal { get; set; }
    }
}
