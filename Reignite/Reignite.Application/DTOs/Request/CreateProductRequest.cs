using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reignite.Application.DTOs.Request
{
    public class CreateProductRequest
    {
        [Required(ErrorMessage = "Naziv proizvoda je obavezan.")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Naziv mora imati između 2 i 200 znakova.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Cijena je obavezna.")]
        [Range(0.01, 1000000, ErrorMessage = "Cijena mora biti veća od 0.")]
        public decimal Price { get; set; }

        [StringLength(2000, ErrorMessage = "Opis ne smije biti duži od 2000 znakova.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Kategorija proizvoda je obavezna.")]
        [Range(1, int.MaxValue, ErrorMessage = "Kategorija proizvoda mora biti ispravna.")]
        public int ProductCategoryId { get; set; }

        [Required(ErrorMessage = "Dobavljač je obavezan.")]
        [Range(1, int.MaxValue, ErrorMessage = "Dobavljač mora biti ispravan.")]
        public int SupplierId { get; set; }
        public string? ProductImageUrl { get; set; }
    }
}
