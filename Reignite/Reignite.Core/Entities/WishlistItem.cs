using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reignite.Core.Entities
{
    public class WishlistItem:BaseEntity
    {
        public int WishlistId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        // Navigation properties
        public Wishlist Wishlist { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}

