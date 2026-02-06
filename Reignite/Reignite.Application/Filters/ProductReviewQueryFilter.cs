using Reignite.Application.Common;

namespace Reignite.Application.Filters
{
    public class ProductReviewQueryFilter : PaginationRequest
    {
        public int? ProductId { get; set; }
        public int? UserId { get; set; }
        public int? MinRating { get; set; }
        public int? MaxRating { get; set; }
        public string? OrderBy { get; set; }
    }
}
