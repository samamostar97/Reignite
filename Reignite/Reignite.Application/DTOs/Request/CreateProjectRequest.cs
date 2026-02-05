using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reignite.Application.DTOs.Request
{
    public class CreateProjectRequest
    {
        [Required(ErrorMessage = "Naslov projekta je obavezan.")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Naslov mora imati između 2 i 200 znakova.")]
        public string Title { get; set; } = string.Empty;

        [StringLength(2000, ErrorMessage = "Opis ne smije biti duži od 2000 znakova.")]
        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        [Range(0, 10000, ErrorMessage = "Broj sati mora biti između 0 i 10000.")]
        public int? HoursSpent { get; set; }

        [Required(ErrorMessage = "Korisnik je obavezan.")]
        [Range(1, int.MaxValue, ErrorMessage = "Korisnik mora biti ispravan.")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Hobi je obavezan.")]
        [Range(1, int.MaxValue, ErrorMessage = "Hobi mora biti ispravan.")]
        public int HobbyId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Proizvod mora biti ispravan.")]
        public int? ProductId { get; set; }
    }
}
