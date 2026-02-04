using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reignite.Core.Entities
{
    public class ProductCategory : BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        // Navigation properties
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}

