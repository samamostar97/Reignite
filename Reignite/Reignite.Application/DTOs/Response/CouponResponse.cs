namespace Reignite.Application.DTOs.Response
{
    public class CouponResponse
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string DiscountType { get; set; } = string.Empty;
        public decimal DiscountValue { get; set; }
        public decimal? MinimumOrderAmount { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public int? MaxUses { get; set; }
        public int TimesUsed { get; set; }
        public bool IsActive { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsExpired => ExpiryDate.HasValue && ExpiryDate.Value < DateTime.UtcNow;
        public bool IsMaxedOut => MaxUses.HasValue && TimesUsed >= MaxUses.Value;
    }
}
