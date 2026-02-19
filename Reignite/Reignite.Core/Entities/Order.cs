using Reignite.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reignite.Core.Entities
{
    public class Order : BaseEntity
    {
        public int UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime PurchaseDate { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Processing;
        public string? StripePaymentId { get; set; }
        public string? CouponCode { get; set; }
        public decimal DiscountAmount { get; set; }

        // Navigation properties
        public User User { get; set; } = null!;
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}

