using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Reignite.Application.Common;
using Reignite.Application.DTOs.Request;
using Reignite.Application.DTOs.Response;
using Reignite.Application.Filters;
using Reignite.Application.IRepositories;
using Reignite.Application.IServices;
using Reignite.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reignite.Infrastructure.Services
{
    public class ProjectService : BaseService<Project, ProjectResponse, CreateProjectRequest, UpdateProjectRequest, ProjectQueryFilter, int>, IProjectService
    {
        private readonly IRepository<Project, int> _projectRepository;
        private readonly IMapper _mapper;
        private readonly IFileStorageService _fileStorageService;

        public ProjectService(IRepository<Project, int> repository, IMapper mapper, IFileStorageService fileStorageService) : base(repository, mapper)
        {
            _projectRepository = repository;
            _mapper = mapper;
            _fileStorageService = fileStorageService;
        }

        public override async Task<ProjectResponse> GetByIdAsync(int id)
        {
            var project = await _projectRepository.AsQueryable()
                .Include(p => p.User)
                .Include(p => p.Hobby)
                .Include(p => p.Product)
                .Include(p => p.Reviews)
                    .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
                throw new KeyNotFoundException("Projekat nije pronađen");

            return _mapper.Map<ProjectResponse>(project);
        }

        public async Task<PagedResult<ProjectResponse>> GetTopRatedProjectsAsync(int pageNumber = 1, int pageSize = 3)
        {
            var query = _projectRepository.AsQueryable()
                .Include(p => p.User)
                .Include(p => p.Hobby)
                .Include(p => p.Product)
                .Include(p => p.Reviews)
                .Where(p => p.Reviews.Any())
                .Select(p => new
                {
                    Project = p,
                    AverageRating = p.Reviews.Average(r => r.Rating),
                    ReviewCount = p.Reviews.Count()
                })
                .OrderByDescending(x => x.AverageRating)
                .ThenByDescending(x => x.ReviewCount);

            var totalCount = await query.CountAsync();

            var projects = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var items = projects.Select(x =>
            {
                var response = _mapper.Map<ProjectResponse>(x.Project);
                response.AverageRating = x.AverageRating;
                response.ReviewCount = x.ReviewCount;
                return response;
            }).ToList();

            return new PagedResult<ProjectResponse>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber
            };
        }

        public async Task<ProjectResponse> UploadImageAsync(int projectId, FileUploadRequest fileRequest)
        {
            var project = await _projectRepository.AsQueryable()
                .Include(x => x.User)
                .Include(x => x.Hobby)
                .Include(x => x.Product)
                .Include(x => x.Reviews)
                .FirstOrDefaultAsync(x => x.Id == projectId);

            if (project == null)
                throw new KeyNotFoundException("Projekat nije pronađen");

            if (!string.IsNullOrEmpty(project.ImageUrl))
            {
                await _fileStorageService.DeleteAsync(project.ImageUrl);
            }

            var uploadResult = await _fileStorageService.UploadAsync(fileRequest, "projects", projectId.ToString());

            if (!uploadResult.Success)
                throw new InvalidOperationException(uploadResult.ErrorMessage);

            project.ImageUrl = uploadResult.FileUrl;
            await _projectRepository.UpdateAsync(project);

            return _mapper.Map<ProjectResponse>(project);
        }

        public async Task<bool> DeleteImageAsync(int projectId)
        {
            var project = await _projectRepository.GetByIdAsync(projectId);

            if (project == null)
                throw new KeyNotFoundException("Projekat nije pronađen");

            if (string.IsNullOrEmpty(project.ImageUrl))
                return false;

            var deleted = await _fileStorageService.DeleteAsync(project.ImageUrl);

            project.ImageUrl = null;
            await _projectRepository.UpdateAsync(project);

            return deleted;
        }

        protected override IQueryable<Project> ApplyFilter(IQueryable<Project> query, ProjectQueryFilter filter)
        {
            query = query.Include(x => x.User).Include(x => x.Hobby).Include(x => x.Product).Include(x => x.Reviews);

            if (!string.IsNullOrEmpty(filter.Search))
                query = query.Where(x => x.Title.ToLower().Contains(filter.Search.ToLower()));

            if (filter.HobbyId.HasValue)
                query = query.Where(x => x.HobbyId == filter.HobbyId.Value);

            if (!string.IsNullOrEmpty(filter.OrderBy))
            {
                query = filter.OrderBy.ToLower() switch
                {
                    "title" => query.OrderBy(x => x.Title),
                    "createdatdesc" => query.OrderByDescending(x => x.CreatedAt),
                    _ => query.OrderByDescending(x => x.CreatedAt)
                };
                return query;
            }

            query = query.OrderByDescending(x => x.CreatedAt);
            return query;
        }

        protected override async Task BeforeCreateAsync(Project entity, CreateProjectRequest dto)
        {
            // Check if user already has a project with the same title
            var exists = await _projectRepository.AsQueryable()
                .AnyAsync(p => p.UserId == entity.UserId &&
                              p.Title.ToLower() == entity.Title.ToLower());

            if (exists)
                throw new InvalidOperationException("Već imate projekat sa istim nazivom");
        }

        protected override async Task BeforeUpdateAsync(Project entity, UpdateProjectRequest dto)
        {
            // Only check if title is being changed
            if (!string.IsNullOrEmpty(dto.Title))
            {
                var exists = await _projectRepository.AsQueryable()
                    .AnyAsync(p => p.Id != entity.Id &&
                                  p.UserId == entity.UserId &&
                                  p.Title.ToLower() == dto.Title.ToLower());

                if (exists)
                    throw new InvalidOperationException("Već imate projekat sa istim nazivom");
            }
        }

        protected override async Task AfterCreateAsync(Project entity, CreateProjectRequest dto)
        {
            await LoadNavigationPropertiesAsync(entity);
        }

        protected override async Task AfterUpdateAsync(Project entity, UpdateProjectRequest dto)
        {
            await LoadNavigationPropertiesAsync(entity);
        }

        private async Task LoadNavigationPropertiesAsync(Project entity)
        {
            // Reload entity with all navigation properties for proper mapping
            var loaded = await _projectRepository.AsQueryable()
                .Include(p => p.User)
                .Include(p => p.Hobby)
                .Include(p => p.Product)
                .Include(p => p.Reviews)
                .FirstOrDefaultAsync(p => p.Id == entity.Id);

            if (loaded != null)
            {
                entity.User = loaded.User;
                entity.Hobby = loaded.Hobby;
                entity.Product = loaded.Product;
                entity.Reviews = loaded.Reviews;
            }
        }
    }
}
