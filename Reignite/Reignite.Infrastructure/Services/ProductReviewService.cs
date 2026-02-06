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
    public class ProductReviewService : BaseService<ProductReview, ProductReviewResponse, CreateProductReviewRequest, UpdateProductReviewRequest, ProductReviewQueryFilter, int>, IProductReviewService
    {
        public ProductReviewService(
            IRepository<ProductReview, int> repository,
            IMapper mapper) : base(repository, mapper)
        {
        }

        protected override async Task BeforeCreateAsync(ProductReview entity, CreateProductReviewRequest dto)
        {
            if (entity.UserId > 0)
            {
                var exists = await _repository.AsQueryable()
                    .AnyAsync(r => r.UserId == entity.UserId && r.ProductId == entity.ProductId);

                if (exists)
                    throw new InvalidOperationException("Već ste ostavili recenziju za ovaj proizvod.");
            }
        }

        public async Task<ProductReviewResponse> CreateForUserAsync(int userId, CreateProductReviewRequest dto)
        {
            var existingReview = await _repository.AsQueryable()
                .AnyAsync(r => r.UserId == userId && r.ProductId == dto.ProductId);

            if (existingReview)
                throw new InvalidOperationException("Već ste ostavili recenziju za ovaj proizvod.");

            var review = new ProductReview
            {
                UserId = userId,
                ProductId = dto.ProductId,
                Rating = dto.Rating,
                Comment = dto.Comment
            };

            await _repository.AddAsync(review);

            var result = await _repository.AsQueryable()
                .Include(r => r.User)
                .Include(r => r.Product)
                .FirstOrDefaultAsync(r => r.Id == review.Id);

            return MapToResponse(result!);
        }

        public async Task<PagedResult<ProductReviewResponse>> GetByProductIdAsync(int productId, PaginationRequest pagination)
        {
            var query = _repository.AsQueryable()
                .Include(r => r.User)
                .Include(r => r.Product)
                .Where(r => r.ProductId == productId)
                .OrderByDescending(r => r.CreatedAt);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();

            return new PagedResult<ProductReviewResponse>
            {
                Items = items.Select(MapToResponse).ToList(),
                TotalCount = totalCount,
                PageNumber = pagination.PageNumber
            };
        }

        public async Task<double> GetAverageRatingAsync(int productId)
        {
            var reviews = await _repository.AsQueryable()
                .Where(r => r.ProductId == productId)
                .ToListAsync();

            if (!reviews.Any())
                return 0;

            return reviews.Average(r => r.Rating);
        }

        public async Task<bool> HasUserReviewedAsync(int userId, int productId)
        {
            return await _repository.AsQueryable()
                .AnyAsync(r => r.UserId == userId && r.ProductId == productId);
        }

        protected override IQueryable<ProductReview> ApplyFilter(IQueryable<ProductReview> query, ProductReviewQueryFilter filter)
        {
            query = query.Include(r => r.User).Include(r => r.Product);

            if (filter.ProductId.HasValue)
                query = query.Where(r => r.ProductId == filter.ProductId.Value);

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

        public override async Task<ProductReviewResponse> GetByIdAsync(int id)
        {
            var entity = await _repository.AsQueryable()
                .Include(r => r.User)
                .Include(r => r.Product)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (entity == null)
                throw new KeyNotFoundException($"Recenzija sa id '{id}' nije pronađena.");

            return MapToResponse(entity);
        }

        public override async Task<PagedResult<ProductReviewResponse>> GetPagedAsync(ProductReviewQueryFilter filter)
        {
            var query = _repository.AsQueryable();
            query = ApplyFilter(query, filter);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PagedResult<ProductReviewResponse>
            {
                Items = items.Select(MapToResponse).ToList(),
                TotalCount = totalCount,
                PageNumber = filter.PageNumber
            };
        }

        protected override async Task AfterCreateAsync(ProductReview entity, CreateProductReviewRequest dto)
        {
            var loaded = await _repository.AsQueryable()
                .Include(r => r.User)
                .Include(r => r.Product)
                .FirstOrDefaultAsync(r => r.Id == entity.Id);

            if (loaded != null)
            {
                entity.User = loaded.User;
                entity.Product = loaded.Product;
            }
        }

        protected override async Task AfterUpdateAsync(ProductReview entity, UpdateProductReviewRequest dto)
        {
            var loaded = await _repository.AsQueryable()
                .Include(r => r.User)
                .Include(r => r.Product)
                .FirstOrDefaultAsync(r => r.Id == entity.Id);

            if (loaded != null)
            {
                entity.User = loaded.User;
                entity.Product = loaded.Product;
            }
        }

        private static ProductReviewResponse MapToResponse(ProductReview review)
        {
            return new ProductReviewResponse
            {
                Id = review.Id,
                UserId = review.UserId,
                UserName = review.User != null ? $"{review.User.FirstName} {review.User.LastName}" : "Nepoznat korisnik",
                UserProfileImageUrl = review.User?.ProfileImageUrl,
                ProductId = review.ProductId,
                ProductName = review.Product?.Name ?? "Nepoznat proizvod",
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedAt = review.CreatedAt
            };
        }
    }
}
