using Reignite.Application.Common;
using Reignite.Application.DTOs.Response;

namespace Reignite.Application.Filters
{
    public class ActivityQueryFilter : PaginationRequest
    {
        public ActivityType? Type { get; set; }
    }
}
