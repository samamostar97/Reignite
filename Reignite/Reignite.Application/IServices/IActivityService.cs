using Reignite.Application.Common;
using Reignite.Application.DTOs.Response;
using Reignite.Application.Filters;

namespace Reignite.Application.IServices
{
    public interface IActivityService
    {
        Task<PagedResult<ActivityResponse>> GetPagedAsync(ActivityQueryFilter filter);
    }
}
