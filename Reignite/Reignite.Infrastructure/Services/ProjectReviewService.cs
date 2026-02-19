using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Reignite.Application.Common;
using Reignite.Application.DTOs.Request;
using Reignite.Application.DTOs.Response;
using Reignite.Application.Filters;
using Reignite.Application.IRepositories;
using Reignite.Application.IServices;
using Reignite.Core.Entities;

namespace Reignite.Infrastructure.Services
{
    public class ProjectReviewService : BaseService<ProjectReview, ProjectReviewResponse, CreateProjectReviewRequest, UpdateProjectReviewRequest, ProjectReviewQueryFilter, int>, IProjectReviewService
    {
        public ProjectReviewService(
            IRepository<ProjectReview, int> repository,
            IMapper mapper) : base(repository, mapper)
        {
        }

        protected override async Task BeforeCreateAsync(ProjectReview entity, CreateProjectReviewRequest dto, CancellationToken cancellationToken = default)
        {
            if (entity.UserId > 0)
            {
                var exists = await _repository.AsQueryable()
                    .AnyAsync(r => r.UserId == entity.UserId && r.ProjectId == entity.ProjectId, cancellationToken);

                if (exists)
                    throw new InvalidOperationException("Već ste ostavili recenziju za ovaj projekat.");
            }
        }

        public async Task<ProjectReviewResponse> CreateForUserAsync(int userId, CreateProjectReviewRequest dto, CancellationToken cancellationToken = default)
        {
            var existingReview = await _repository.AsQueryable()
                .AnyAsync(r => r.UserId == userId && r.ProjectId == dto.ProjectId, cancellationToken);

            if (existingReview)
                throw new InvalidOperationException("Već ste ostavili recenziju za ovaj projekat.");

            var review = new ProjectReview
            {
                UserId = userId,
                ProjectId = dto.ProjectId,
                Rating = dto.Rating,
                Comment = dto.Comment
            };

            await _repository.AddAsync(review, cancellationToken);

            var result = await _repository.AsQueryable()
                .Include(r => r.User)
                .Include(r => r.Project)
                .FirstOrDefaultAsync(r => r.Id == review.Id, cancellationToken);

            return MapToResponse(result!);
        }

        public async Task<PagedResult<ProjectReviewResponse>> GetByProjectIdAsync(int projectId, PaginationRequest pagination, CancellationToken cancellationToken = default)
        {
            var query = _repository.AsQueryable()
                .Include(r => r.User)
                .Include(r => r.Project)
                .Where(r => r.ProjectId == projectId)
                .OrderByDescending(r => r.CreatedAt);

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<ProjectReviewResponse>
            {
                Items = items.Select(MapToResponse).ToList(),
                TotalCount = totalCount,
                PageNumber = pagination.PageNumber
            };
        }

        public async Task<double> GetAverageRatingAsync(int projectId, CancellationToken cancellationToken = default)
        {
            var reviews = await _repository.AsQueryable()
                .Where(r => r.ProjectId == projectId)
                .ToListAsync(cancellationToken);

            if (!reviews.Any())
                return 0;

            return reviews.Average(r => r.Rating);
        }

        public async Task<bool> HasUserReviewedAsync(int userId, int projectId, CancellationToken cancellationToken = default)
        {
            return await _repository.AsQueryable()
                .AnyAsync(r => r.UserId == userId && r.ProjectId == projectId, cancellationToken);
        }

        protected override IQueryable<ProjectReview> ApplyFilter(IQueryable<ProjectReview> query, ProjectReviewQueryFilter filter)
        {
            query = query.Include(r => r.User).Include(r => r.Project);

            if (filter.ProjectId.HasValue)
                query = query.Where(r => r.ProjectId == filter.ProjectId.Value);

            if (filter.UserId.HasValue)
                query = query.Where(r => r.UserId == filter.UserId.Value);

            if (filter.MinRating.HasValue)
                query = query.Where(r => r.Rating >= filter.MinRating.Value);

            if (filter.MaxRating.HasValue)
                query = query.Where(r => r.Rating <= filter.MaxRating.Value);

            if (!string.IsNullOrEmpty(filter.OrderBy))
            {
                query = filter.OrderBy.ToLower() switch
                {
                    "rating" => query.OrderBy(r => r.Rating),
                    "ratingdesc" => query.OrderByDescending(r => r.Rating),
                    "createdatdesc" => query.OrderByDescending(r => r.CreatedAt),
                    _ => query.OrderByDescending(r => r.CreatedAt)
                };
                return query;
            }

            return query.OrderByDescending(r => r.CreatedAt);
        }

        public override async Task<ProjectReviewResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var entity = await _repository.AsQueryable()
                .Include(r => r.User)
                .Include(r => r.Project)
                .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

            if (entity == null)
                throw new KeyNotFoundException($"Recenzija sa id '{id}' nije pronađena.");

            return MapToResponse(entity);
        }

        public override async Task<PagedResult<ProjectReviewResponse>> GetPagedAsync(ProjectReviewQueryFilter filter, CancellationToken cancellationToken = default)
        {
            var query = _repository.AsQueryable();
            query = ApplyFilter(query, filter);

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<ProjectReviewResponse>
            {
                Items = items.Select(MapToResponse).ToList(),
                TotalCount = totalCount,
                PageNumber = filter.PageNumber
            };
        }

        protected override async Task AfterCreateAsync(ProjectReview entity, CreateProjectReviewRequest dto, CancellationToken cancellationToken = default)
        {
            var loaded = await _repository.AsQueryable()
                .Include(r => r.User)
                .Include(r => r.Project)
                .FirstOrDefaultAsync(r => r.Id == entity.Id, cancellationToken);

            if (loaded != null)
            {
                entity.User = loaded.User;
                entity.Project = loaded.Project;
            }
        }

        protected override async Task AfterUpdateAsync(ProjectReview entity, UpdateProjectReviewRequest dto, CancellationToken cancellationToken = default)
        {
            var loaded = await _repository.AsQueryable()
                .Include(r => r.User)
                .Include(r => r.Project)
                .FirstOrDefaultAsync(r => r.Id == entity.Id, cancellationToken);

            if (loaded != null)
            {
                entity.User = loaded.User;
                entity.Project = loaded.Project;
            }
        }

        private static ProjectReviewResponse MapToResponse(ProjectReview review)
        {
            return new ProjectReviewResponse
            {
                Id = review.Id,
                UserId = review.UserId,
                UserName = review.User != null ? $"{review.User.FirstName} {review.User.LastName}" : "Nepoznat korisnik",
                UserProfileImageUrl = review.User?.ProfileImageUrl,
                ProjectId = review.ProjectId,
                ProjectName = review.Project?.Title ?? "Nepoznat projekat",
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedAt = review.CreatedAt
            };
        }
    }
}
