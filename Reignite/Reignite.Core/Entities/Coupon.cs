using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reignite.Core.Entities
{
    public class Coupon : BaseEntity
    {
        public string Code { get; set; } = string.Empty; 
        public string DiscountType { get; set; } = string.Empty; 
        public decimal DiscountValue { get; set; }
        public decimal? MinimumOrderAmount { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public int? MaxUses { get; set; }
        public int TimesUsed { get; set; }
        public bool IsActive { get; set; }
        public bool IsFeatured { get; set; }
    }
}

