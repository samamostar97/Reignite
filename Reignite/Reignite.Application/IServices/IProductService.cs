using Reignite.Application.Common;
using Reignite.Application.DTOs.Request;
using Reignite.Application.DTOs.Response;
using Reignite.Application.Filters;
using Reignite.Core.Entities;


namespace Reignite.Application.IServices
{
    public interface IProductService:IService<Product,ProductResponse,CreateProductRequest,UpdateProductRequest,ProductQueryFilter,int>
    {
        Task<ProductResponse> CreateWithImageAsync(CreateProductRequest dto, FileUploadRequest? imageRequest, CancellationToken cancellationToken = default);
        Task<ProductResponse> UploadImageAsync(int productId, FileUploadRequest fileRequest, CancellationToken cancellationToken = default);
        Task<bool> DeleteImageAsync(int productId, CancellationToken cancellationToken = default);
        Task<List<ProductResponse>> GetBestSellingAsync(int count = 5, CancellationToken cancellationToken = default);
    }
}
