using Reignite.Application.Common;

namespace Reignite.Application.Filters
{
    public class FaqQueryFilter : PaginationRequest
    {
        public string? Search { get; set; }
        public string? OrderBy { get; set; }
    }
}
