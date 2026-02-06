using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reignite.Core.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? Description { get; set; }

        public int ProductCategoryId { get; set; }
        public int SupplierId { get; set; }
        public string? ProductImageUrl { get; set; }


        // Navigation properties
        public ProductCategory ProductCategory { get; set; } = null!;
        public Supplier Supplier { get; set; } = null!;
        public ICollection<ProductReview> ProductReviews { get; set; } = new List<ProductReview>();
        public ICollection<Project> Projects { get; set; } = new List<Project>();
    }
}

