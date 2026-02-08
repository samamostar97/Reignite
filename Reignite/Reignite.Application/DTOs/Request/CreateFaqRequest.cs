using System.ComponentModel.DataAnnotations;

namespace Reignite.Application.DTOs.Request
{
    public class CreateFaqRequest
    {
        [Required(ErrorMessage = "Pitanje je obavezno.")]
        [StringLength(500, ErrorMessage = "Pitanje ne smije biti duže od 500 karaktera.")]
        public string Question { get; set; } = string.Empty;

        [Required(ErrorMessage = "Odgovor je obavezan.")]
        [StringLength(2000, ErrorMessage = "Odgovor ne smije biti duži od 2000 karaktera.")]
        public string Answer { get; set; } = string.Empty;
    }
}
