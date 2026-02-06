using Reignite.Application.Common;
using Reignite.Application.DTOs.Request;
using Reignite.Application.DTOs.Response;
using Reignite.Application.Filters;
using Reignite.Core.Entities;

namespace Reignite.Application.IServices
{
    public interface IProductReviewService : IService<ProductReview, ProductReviewResponse, CreateProductReviewRequest, UpdateProductReviewRequest, ProductReviewQueryFilter, int>
    {
        Task<ProductReviewResponse> CreateForUserAsync(int userId, CreateProductReviewRequest dto);
        Task<PagedResult<ProductReviewResponse>> GetByProductIdAsync(int productId, PaginationRequest pagination);
        Task<double> GetAverageRatingAsync(int productId);
        Task<bool> HasUserReviewedAsync(int userId, int productId);
    }
}
