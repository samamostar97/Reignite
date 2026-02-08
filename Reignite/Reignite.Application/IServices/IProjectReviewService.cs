using Reignite.Application.Common;
using Reignite.Application.DTOs.Request;
using Reignite.Application.DTOs.Response;
using Reignite.Application.Filters;
using Reignite.Core.Entities;

namespace Reignite.Application.IServices
{
    public interface IProjectReviewService : IService<ProjectReview, ProjectReviewResponse, CreateProjectReviewRequest, UpdateProjectReviewRequest, ProjectReviewQueryFilter, int>
    {
        Task<ProjectReviewResponse> CreateForUserAsync(int userId, CreateProjectReviewRequest dto);
        Task<PagedResult<ProjectReviewResponse>> GetByProjectIdAsync(int projectId, PaginationRequest pagination);
        Task<double> GetAverageRatingAsync(int projectId);
        Task<bool> HasUserReviewedAsync(int userId, int projectId);
    }
}
