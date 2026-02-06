using Microsoft.EntityFrameworkCore;
using Reignite.Application.Common;
using Reignite.Application.DTOs.Response;
using Reignite.Application.Filters;
using Reignite.Application.IRepositories;
using Reignite.Application.IServices;
using Reignite.Core.Entities;

namespace Reignite.Infrastructure.Services
{
    public class ActivityService : IActivityService
    {
        private readonly IRepository<Review, int> _reviewRepository;
        private readonly IRepository<ProjectReview, int> _projectReviewRepository;
        private readonly IRepository<Project, int> _projectRepository;

        public ActivityService(
            IRepository<Review, int> reviewRepository,
            IRepository<ProjectReview, int> projectReviewRepository,
            IRepository<Project, int> projectRepository)
        {
            _reviewRepository = reviewRepository;
            _projectReviewRepository = projectReviewRepository;
            _projectRepository = projectRepository;
        }

        public async Task<PagedResult<ActivityResponse>> GetPagedAsync(ActivityQueryFilter filter)
        {
            var activities = new List<ActivityResponse>();

            // Only fetch types that match the filter (or all if no filter)
            var includeProductReviews = !filter.Type.HasValue || filter.Type == ActivityType.ProductReview;
            var includeProjectReviews = !filter.Type.HasValue || filter.Type == ActivityType.ProjectReview;
            var includeNewProjects = !filter.Type.HasValue || filter.Type == ActivityType.NewProject;

            if (includeProductReviews)
            {
                var productReviews = await _reviewRepository.AsQueryable()
                    .Include(r => r.User)
                    .Include(r => r.Product)
                    .OrderByDescending(r => r.CreatedAt)
                    .ToListAsync();

                activities.AddRange(productReviews.Select(r => new ActivityResponse
                {
                    Id = r.Id,
                    Type = ActivityType.ProductReview,
                    Title = "Nova recenzija proizvoda",
                    Description = r.Comment ?? string.Empty,
                    Rating = r.Rating,
                    UserName = $"{r.User.FirstName} {r.User.LastName}",
                    UserProfileImageUrl = r.User.ProfileImageUrl,
                    TargetName = r.Product.Name,
                    TargetImageUrl = r.Product.ProductImageUrl,
                    TargetId = r.ProductId,
                    CreatedAt = r.CreatedAt
                }));
            }

            if (includeProjectReviews)
            {
                var projectReviews = await _projectReviewRepository.AsQueryable()
                    .Include(r => r.User)
                    .Include(r => r.Project)
                    .OrderByDescending(r => r.CreatedAt)
                    .ToListAsync();

                activities.AddRange(projectReviews.Select(r => new ActivityResponse
                {
                    Id = r.Id,
                    Type = ActivityType.ProjectReview,
                    Title = "Nova recenzija projekta",
                    Description = r.Comment ?? string.Empty,
                    Rating = r.Rating,
                    UserName = $"{r.User.FirstName} {r.User.LastName}",
                    UserProfileImageUrl = r.User.ProfileImageUrl,
                    TargetName = r.Project.Title,
                    TargetImageUrl = r.Project.ImageUrl,
                    TargetId = r.ProjectId,
                    CreatedAt = r.CreatedAt
                }));
            }

            if (includeNewProjects)
            {
                var projects = await _projectRepository.AsQueryable()
                    .Include(p => p.User)
                    .Include(p => p.Hobby)
                    .OrderByDescending(p => p.CreatedAt)
                    .ToListAsync();

                activities.AddRange(projects.Select(p => new ActivityResponse
                {
                    Id = p.Id,
                    Type = ActivityType.NewProject,
                    Title = "Novi projekat",
                    Description = p.Title,
                    Rating = null,
                    UserName = $"{p.User.FirstName} {p.User.LastName}",
                    UserProfileImageUrl = p.User.ProfileImageUrl,
                    TargetName = p.Hobby.Name,
                    TargetImageUrl = p.ImageUrl,
                    TargetId = p.Id,
                    CreatedAt = p.CreatedAt
                }));
            }

            // Sort all activities by date
            var sorted = activities.OrderByDescending(a => a.CreatedAt).ToList();
            var totalCount = sorted.Count;

            // Apply paging
            var paged = sorted
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();

            return new PagedResult<ActivityResponse>
            {
                Items = paged,
                TotalCount = totalCount,
                PageNumber = filter.PageNumber
            };
        }
    }
}
