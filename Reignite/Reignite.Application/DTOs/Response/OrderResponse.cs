using Reignite.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reignite.Application.DTOs.Response
{
    public class OrderResponse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string? UserProfileImageUrl { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime PurchaseDate { get; set; }
        public OrderStatus Status { get; set; }
        public string StatusName => Status.ToString();
        public string? StripePaymentId { get; set; }
        public string? CouponCode { get; set; }
        public decimal DiscountAmount { get; set; }
        public int ItemCount { get; set; }
        public List<OrderItemResponse> Items { get; set; } = new();
    }
}
