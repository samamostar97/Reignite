using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reignite.Core.Entities
{
    public class OrderItem : BaseEntity
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        // Navigation properties
        public Order Order { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}

