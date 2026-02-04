using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reignite.Core.Entities
{
    public class Supplier : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Website { get; set; }

        // Navigation property
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}

