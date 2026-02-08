using System.ComponentModel.DataAnnotations;

namespace Reignite.Application.DTOs.Request
{
    public class UpdateCouponRequest
    {
        [Required(ErrorMessage = "Kod kupona je obavezan.")]
        [StringLength(50, ErrorMessage = "Kod kupona ne smije biti duži od 50 karaktera.")]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tip popusta je obavezan.")]
        public string DiscountType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vrijednost popusta je obavezna.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Vrijednost popusta mora biti veća od 0.")]
        public decimal DiscountValue { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Minimalni iznos narudžbe mora biti veći ili jednak 0.")]
        public decimal? MinimumOrderAmount { get; set; }

        public DateTime? ExpiryDate { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Maksimalan broj korištenja mora biti veći od 0.")]
        public int? MaxUses { get; set; }

        public bool IsActive { get; set; }
    }
}
