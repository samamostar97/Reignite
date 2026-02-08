using Reignite.Application.Common;

namespace Reignite.Application.Filters
{
    public class CouponQueryFilter : PaginationRequest
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsExpired { get; set; }
        public string? OrderBy { get; set; }
    }
}
