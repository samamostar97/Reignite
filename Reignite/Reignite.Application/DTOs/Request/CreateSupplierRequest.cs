using System.ComponentModel.DataAnnotations;

namespace Reignite.Application.DTOs.Request
{
    public class CreateSupplierRequest
    {
        [Required(ErrorMessage = "Naziv dobavljača je obavezan.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Naziv mora imati između 2 i 100 znakova.")]
        public string Name { get; set; } = string.Empty;

        [Url(ErrorMessage = "Website mora biti validan URL.")]
        [StringLength(200, ErrorMessage = "Website može imati najviše 200 znakova.")]
        public string? Website { get; set; }
    }
}
