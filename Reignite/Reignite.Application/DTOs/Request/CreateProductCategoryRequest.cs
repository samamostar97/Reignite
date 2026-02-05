using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reignite.Application.DTOs.Request
{
    public class CreateProductCategoryRequest
    {
        [Required(ErrorMessage = "Naziv kategorije je obavezan.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Naziv mora imati između 2 i 100 znakova.")]
        public string Name { get; set; } = string.Empty;
    }
}
