using System.ComponentModel.DataAnnotations;

namespace Reignite.Application.DTOs.Request
{
    public class UpdateHobbyRequest
    {
        [Required(ErrorMessage = "Naziv hobija je obavezan.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Naziv mora imati izmedju 2 i 100 znakova.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Opis moze imati najvise 500 znakova.")]
        public string? Description { get; set; }
    }
}
