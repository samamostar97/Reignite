using Reignite.Application.Common;
using Reignite.Application.DTOs.Request;
using Reignite.Application.DTOs.Response;
using Reignite.Application.Filters;
using Reignite.Core.Entities;

namespace Reignite.Application.IServices
{
    public interface IProjectService : IService<Project, ProjectResponse, CreateProjectRequest, UpdateProjectRequest, ProjectQueryFilter, int>
    {
        Task<PagedResult<ProjectResponse>> GetTopRatedProjectsAsync(int pageNumber = 1, int pageSize = 3);
        Task<ProjectResponse> UploadImageAsync(int projectId, FileUploadRequest fileRequest);
        Task<bool> DeleteImageAsync(int projectId);
        Task<bool> IsOwnerAsync(int projectId, int userId);
    }
}
