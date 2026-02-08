using Reignite.Application.Common;

namespace Reignite.Application.Filters
{
    public class ProjectReviewQueryFilter : PaginationRequest
    {
        public int? ProjectId { get; set; }
        public int? UserId { get; set; }
        public int? MinRating { get; set; }
        public int? MaxRating { get; set; }
        public string? OrderBy { get; set; }
    }
}
