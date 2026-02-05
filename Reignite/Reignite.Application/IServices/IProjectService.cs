using Reignite.Application.DTOs.Request;
using Reignite.Application.DTOs.Response;
using Reignite.Application.Filters;
using Reignite.Core.Entities;

namespace Reignite.Application.IServices
{
    public interface IProjectService : IService<Project, ProjectResponse, CreateProjectRequest, UpdateProjectRequest, ProjectQueryFilter, int>
    {
        Task<List<ProjectResponse>> GetTopRatedProjectsAsync(int count = 3);
    }
}
